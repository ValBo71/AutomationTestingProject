import http from 'k6/http';
import { check, sleep } from 'k6';
import exec from 'k6/execution';

// Read environment variables with defaults
const USERS = parseInt(__ENV.USERS || '20', 10);
const RAMPUP = __ENV.RAMPUP || '60s';
const DURATION = __ENV.DURATION || '60s';

// Load test users JSON
const testUsers = JSON.parse(open('./test-users.json'));

export const options = {
  scenarios: {
    main_load: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: RAMPUP, target: USERS },
        { duration: DURATION, target: USERS },
      ],
      gracefulRampDown: '5s',
      exec: 'mainLoadScenario',
    },
    account_lifecycle: {
      executor: 'constant-vus',
      vus: parseInt(__ENV.ACCOUNT_USERS || '2', 10),
      duration: DURATION,
      exec: 'accountLifecycleScenario',
    },
  },
  thresholds: {
    // SLA criteria
    http_req_failed: ['rate<0.05'], // Error Rate < 5%
    'http_req_duration{scenario:main_load}': [
      'avg<2000',   // Average Response Time < 2000 ms
      'p(95)<3000',  // 95th Percentile < 3000 ms
      'p(99)<5000',  // 99th Percentile < 5000 ms
    ],
    'http_req_duration{scenario:account_lifecycle}': [
      'avg<2000',
      'p(95)<3000',
      'p(99)<5000',
    ],
  },
};

const headers = {
  'Content-Type': 'application/x-www-form-urlencoded',
  'Accept': 'application/json',
  'User-Agent': 'k6 Performance Test',
};

// 1. MAIN LOAD SCENARIO
export function mainLoadScenario() {
  const baseUrl = 'https://automationexercise.com';
  
  // GET All Products List
  let resProducts = http.get(`${baseUrl}/api/productsList`, { headers });
  check(resProducts, {
    'GET Products: HTTP Status is 200': (r) => r.status === 200,
    'GET Products: JSON responseCode is 200': (r) => {
      try {
        let json = JSON.parse(r.body);
        return json.responseCode === 200;
      } catch (e) {
        return false;
      }
    },
    'GET Products: Body has products list': (r) => r.body.includes('products'),
  });
  sleep(Math.random() * 1.5 + 0.5); // Think time: 0.5s - 2.0s

  // GET All Brands List
  let resBrands = http.get(`${baseUrl}/api/brandsList`, { headers });
  check(resBrands, {
    'GET Brands: HTTP Status is 200': (r) => r.status === 200,
    'GET Brands: JSON responseCode is 200': (r) => {
      try {
        let json = JSON.parse(r.body);
        return json.responseCode === 200;
      } catch (e) {
        return false;
      }
    },
    'GET Brands: Body has brands list': (r) => r.body.includes('brands'),
  });
  sleep(Math.random() * 1.5 + 0.5);

  // POST Search Product
  const searchProduct = __ENV.SEARCH_PRODUCT || 'tshirt';
  let resSearch = http.post(`${baseUrl}/api/searchProduct`, { search_product: searchProduct }, { headers });
  check(resSearch, {
    'POST Search: HTTP Status is 200': (r) => r.status === 200,
    'POST Search: JSON responseCode is 200': (r) => {
      try {
        let json = JSON.parse(r.body);
        return json.responseCode === 200;
      } catch (e) {
        return false;
      }
    },
    'POST Search: Body contains products': (r) => r.body.includes('products'),
  });
  sleep(Math.random() * 1.5 + 0.5);

  // POST Verify Login
  const user = testUsers[exec.scenario.iterationInInstance % testUsers.length] || testUsers[0];
  let resLogin = http.post(
    `${baseUrl}/api/verifyLogin`,
    { email: user.email, password: user.password },
    { headers }
  );
  check(resLogin, {
    'POST Login: HTTP Status is 200': (r) => r.status === 200,
    'POST Login: JSON responseCode is 200': (r) => {
      try {
        let json = JSON.parse(r.body);
        return json.responseCode === 200;
      } catch (e) {
        return false;
      }
    },
    'POST Login: Body message User exists!': (r) => {
      try {
        let json = JSON.parse(r.body);
        return json.message === 'User exists!';
      } catch (e) {
        return false;
      }
    },
  });
  sleep(Math.random() * 1.5 + 0.5);

  // GET User Detail By Email
  let resUserDetail = http.get(`${baseUrl}/api/getUserDetailByEmail?email=${encodeURIComponent(user.email)}`, { headers });
  check(resUserDetail, {
    'GET UserDetail: HTTP Status is 200': (r) => r.status === 200,
    'GET UserDetail: JSON responseCode is 200': (r) => {
      try {
        let json = JSON.parse(r.body);
        return json.responseCode === 200;
      } catch (e) {
        return false;
      }
    },
    'GET UserDetail: Body contains user': (r) => r.body.includes('user'),
  });
  sleep(Math.random() * 1.5 + 0.5);
}

// 2. ACCOUNT LIFECYCLE SCENARIO
export function accountLifecycleScenario() {
  const baseUrl = 'https://automationexercise.com';
  
  // Generate unique credentials per execution
  const threadNum = exec.vu.idInTest;
  const timestamp = Date.now();
  const randomNum = Math.floor(Math.random() * 1000);
  const uniqueEmail = `performance_k6_${threadNum}_${timestamp}_${randomNum}@abv.bg`;
  const uniqueName = `User_K6_${threadNum}_${timestamp}`;
  const password = 'pass1234';

  // Create Account
  let resCreate = http.post(`${baseUrl}/api/createAccount`, {
    name: uniqueName,
    email: uniqueEmail,
    password: password,
    title: 'Mr',
    birth_date: '01',
    birth_month: '01',
    birth_year: '1990',
    firstname: 'First',
    lastname: 'Last',
    company: 'Company',
    address1: 'Address1',
    address2: 'Address2',
    country: 'Canada',
    zipcode: '12345',
    state: 'State',
    city: 'City',
    mobile_number: '1234567890',
  }, { headers });
  
  check(resCreate, {
    'Create: HTTP Status is 200': (r) => r.status === 200,
    'Create: JSON responseCode is 201': (r) => {
      try {
        let json = JSON.parse(r.body);
        return json.responseCode === 201;
      } catch (e) {
        return false;
      }
    },
    'Create: Body contains User created!': (r) => r.body.includes('User created!'),
  });
  sleep(Math.random() * 2 + 1); // sleep 1s - 3s

  // Get User Details
  let resDetail = http.get(`${baseUrl}/api/getUserDetailByEmail?email=${encodeURIComponent(uniqueEmail)}`, { headers });
  check(resDetail, {
    'GetDetails: HTTP Status is 200': (r) => r.status === 200,
    'GetDetails: JSON responseCode is 200': (r) => {
      try {
        let json = JSON.parse(r.body);
        return json.responseCode === 200;
      } catch (e) {
        return false;
      }
    },
    'GetDetails: Body contains user': (r) => r.body.includes('user'),
  });
  sleep(Math.random() * 2 + 1);

  // Update Account
  let resUpdate = http.put(`${baseUrl}/api/updateAccount`, {
    name: `${uniqueName}_Updated`,
    email: uniqueEmail,
    password: password,
    title: 'Mr',
    birth_date: '02',
    birth_month: '02',
    birth_year: '1991',
    firstname: 'FirstUpdated',
    lastname: 'LastUpdated',
    company: 'CompanyUpdated',
    address1: 'Address1Updated',
    address2: 'Address2Updated',
    country: 'Canada',
    zipcode: '54321',
    state: 'StateUpdated',
    city: 'CityUpdated',
    mobile_number: '0987654321',
  }, { headers });
  
  check(resUpdate, {
    'Update: HTTP Status is 200': (r) => r.status === 200,
    'Update: JSON responseCode is 200': (r) => {
      try {
        let json = JSON.parse(r.body);
        return json.responseCode === 200;
      } catch (e) {
        return false;
      }
    },
    'Update: Body contains User updated!': (r) => r.body.includes('User updated!'),
  });
  sleep(Math.random() * 2 + 1);

  // Delete Account
  let resDelete = http.del(`${baseUrl}/api/deleteAccount`, {
    email: uniqueEmail,
    password: password,
  }, { headers });
  
  check(resDelete, {
    'Delete: HTTP Status is 200': (r) => r.status === 200,
    'Delete: JSON responseCode is 200': (r) => {
      try {
        let json = JSON.parse(r.body);
        return json.responseCode === 200;
      } catch (e) {
        return false;
      }
    },
    'Delete: Body contains Account deleted!': (r) => r.body.includes('Account deleted!'),
  });
  sleep(Math.random() * 2 + 1);
}
