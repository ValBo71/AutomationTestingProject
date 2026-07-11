import http from 'k6/http';
import { check, sleep } from 'k6';
import exec from 'k6/execution';

// Read environment variables with defaults
const BASE_URL = __ENV.BASE_URL || 'https://quickpizza.grafana.com';
const USERS = parseInt(__ENV.USERS || '20', 10);
const RAMPUP = __ENV.RAMPUP || '60s';
const DURATION = __ENV.DURATION || '60s';
const REGISTRATION_USERS = parseInt(__ENV.REGISTRATION_USERS || '5', 10);

// Public demo token documented in Grafana's own QuickPizza k6 examples
// (https://github.com/grafana/quickpizza/blob/main/k6/foundations/01.basic.js) - not a secret.
const DEMO_TOKEN = __ENV.DEMO_TOKEN || 'abcdef0123456789';

const INGREDIENT_TYPES = ['olive_oil', 'tomato', 'mozzarella', 'topping'];

export const options = {
  scenarios: {
    browse_and_recommend: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: RAMPUP, target: USERS },
        { duration: DURATION, target: USERS },
      ],
      gracefulRampDown: '5s',
      exec: 'browseAndRecommendScenario',
    },
    register_and_rate: {
      executor: 'constant-vus',
      vus: REGISTRATION_USERS,
      duration: DURATION,
      exec: 'registerAndRateScenario',
    },
  },
  thresholds: {
    // SLA criteria
    http_req_failed: ['rate<0.05'], // Error Rate < 5%
    'http_req_duration{scenario:browse_and_recommend}': [
      'avg<2000',   // Average Response Time < 2000 ms
      'p(95)<3000', // 95th Percentile < 3000 ms
      'p(99)<5000', // 99th Percentile < 5000 ms
    ],
    'http_req_duration{scenario:register_and_rate}': [
      'avg<2000',
      'p(95)<3000',
      'p(99)<5000',
    ],
  },
};

const jsonHeaders = {
  'Content-Type': 'application/json',
};

function randomRestrictions() {
  return {
    maxCaloriesPerSlice: 500 + Math.floor(Math.random() * 500), // 500-1000
    mustBeVegetarian: Math.random() < 0.3,
    excludedIngredients: [],
    excludedTools: [],
    maxNumberOfToppings: 6,
    minNumberOfToppings: 2,
  };
}

// 1. BROWSE & RECOMMEND SCENARIO
// Simulates a visitor browsing the catalog and getting pizza recommendations,
// authenticated with the shared public demo token.
export function browseAndRecommendScenario() {
  const authHeaders = { ...jsonHeaders, Authorization: `token ${DEMO_TOKEN}` };

  // GET Tools catalog
  let resTools = http.get(`${BASE_URL}/api/tools`, { headers: authHeaders });
  check(resTools, {
    'GET Tools: HTTP Status is 200': (r) => r.status === 200,
    'GET Tools: Body has tools list': (r) => r.body.includes('tools'),
  });
  sleep(Math.random() * 1.5 + 0.5); // Think time: 0.5s - 2.0s

  // GET Doughs catalog
  let resDoughs = http.get(`${BASE_URL}/api/doughs`, { headers: authHeaders });
  check(resDoughs, {
    'GET Doughs: HTTP Status is 200': (r) => r.status === 200,
    'GET Doughs: Body has doughs list': (r) => r.body.includes('doughs'),
  });
  sleep(Math.random() * 1.5 + 0.5);

  // GET Ingredients catalog (random category per iteration)
  const ingredientType = INGREDIENT_TYPES[Math.floor(Math.random() * INGREDIENT_TYPES.length)];
  let resIngredients = http.get(`${BASE_URL}/api/ingredients/${ingredientType}`, { headers: authHeaders });
  check(resIngredients, {
    'GET Ingredients: HTTP Status is 200': (r) => r.status === 200,
    'GET Ingredients: Body has ingredients list': (r) => r.body.includes('ingredients'),
  });
  sleep(Math.random() * 1.5 + 0.5);

  // POST Pizza recommendation
  let resPizza = http.post(`${BASE_URL}/api/pizza`, JSON.stringify(randomRestrictions()), { headers: authHeaders });
  check(resPizza, {
    'POST Pizza: HTTP Status is 200': (r) => r.status === 200,
    'POST Pizza: Body contains a pizza name': (r) => {
      try {
        return typeof r.json('pizza.name') === 'string';
      } catch (e) {
        return false;
      }
    },
  });
  sleep(Math.random() * 1.5 + 0.5);

  // GET Ratings (browse what others rated)
  let resRatings = http.get(`${BASE_URL}/api/ratings`, { headers: authHeaders });
  check(resRatings, {
    'GET Ratings: HTTP Status is 200': (r) => r.status === 200,
    'GET Ratings: Body has ratings list': (r) => r.body.includes('ratings'),
  });
  sleep(Math.random() * 1.5 + 0.5);
}

// 2. REGISTER & RATE SCENARIO
// Simulates the full user lifecycle: register a unique account, log in, get a
// pizza recommendation and rate it.
//
// Note: the pizza `id` returned by POST /api/pizza is not (yet) present in
// QuickPizza's ratable catalog on the public demo, so submitting a rating for
// it consistently returns 400 "pizza ID not found". To keep this scenario
// reliable, ratings are submitted against a known-good seeded pizza id
// (1-50) instead of chaining the freshly recommended id.
//
// No cleanup step: DELETE /api/ratings is rejected on the public demo with
// 403 "operation not permitted for default user" - Grafana deliberately
// blocks bulk-deleting ratings there to protect the shared data set.
export function registerAndRateScenario() {
  const vu = exec.vu.idInInstance;
  const iter = exec.vu.iterationInInstance;
  const uniqueUsername = `k6_qp_${vu}_${iter}_${Date.now()}`.slice(0, 32);
  const password = 'SuperSecret123!';

  // Fetch CSRF token (needed for login)
  let resCsrf = http.post(`${BASE_URL}/api/csrf-token`, null, { headers: jsonHeaders });
  check(resCsrf, { 'CSRF: HTTP Status is 200': (r) => r.status === 200 });
  const csrfToken = resCsrf.cookies.csrf_token ? resCsrf.cookies.csrf_token[0].value : '';
  sleep(0.5);

  // Register a unique user
  let resRegister = http.post(
    `${BASE_URL}/api/users`,
    JSON.stringify({ username: uniqueUsername, password }),
    { headers: jsonHeaders }
  );
  check(resRegister, {
    'Register: HTTP Status is 201': (r) => r.status === 201,
    'Register: Body contains username': (r) => r.body.includes(uniqueUsername),
  });
  sleep(Math.random() * 2 + 1); // sleep 1s - 3s

  // Log in as the new user
  let resLogin = http.post(
    `${BASE_URL}/api/users/token/login`,
    JSON.stringify({ username: uniqueUsername, password, csrf: csrfToken }),
    { headers: jsonHeaders }
  );
  check(resLogin, { 'Login: HTTP Status is 200': (r) => r.status === 200 });

  let token = '';
  try {
    token = resLogin.json('token');
  } catch (e) {
    token = '';
  }
  sleep(Math.random() * 2 + 1);

  if (!token) {
    return; // Can't continue without a token; the failed check above already flags this iteration.
  }
  const authHeaders = { ...jsonHeaders, Authorization: `token ${token}` };

  // Get a pizza recommendation as the logged-in user
  let resPizza = http.post(`${BASE_URL}/api/pizza`, JSON.stringify(randomRestrictions()), { headers: authHeaders });
  check(resPizza, { 'Pizza: HTTP Status is 200': (r) => r.status === 200 });
  sleep(Math.random() * 2 + 1);

  // Rate a known-good seeded pizza (see note above)
  const ratablePizzaId = 1 + Math.floor(Math.random() * 50);
  let resRate = http.post(
    `${BASE_URL}/api/ratings`,
    JSON.stringify({ pizza_id: ratablePizzaId, stars: 1 + Math.floor(Math.random() * 5) }),
    { headers: authHeaders }
  );
  check(resRate, { 'Rate: HTTP Status is 201': (r) => r.status === 201 });
  sleep(Math.random() * 2 + 1);

  // View own ratings
  let resGetRatings = http.get(`${BASE_URL}/api/ratings`, { headers: authHeaders });
  check(resGetRatings, { 'GetRatings: HTTP Status is 200': (r) => r.status === 200 });
  sleep(Math.random() * 2 + 1);
}
