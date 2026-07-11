# Performance Testing Project for QuickPizza

Load testing project for **QuickPizza** (https://quickpizza.grafana.com/), Grafana k6's official
demo/workshop application, built with **Grafana k6**.

---

## 📂 Project Structure

1. **`k6_performance_test.js`** – k6 test script with two scenarios (catalog browsing/recommendations
   and full user registration + rating lifecycle).
2. **`README.md`** – This guide.

---

## 🍕 Scenarios

### 1. Browse & Recommend (`browse_and_recommend`)

Simulates a visitor browsing the QuickPizza catalog and requesting recommendations, authenticated
with the public demo token documented in Grafana's own examples:

1. **GET `/api/tools`** – List available pizza-making tools.
2. **GET `/api/doughs`** – List available dough types.
3. **GET `/api/ingredients/{type}`** – List ingredients for a random category (`olive_oil`, `tomato`,
   `mozzarella`, `topping`).
4. **POST `/api/pizza`** – Get a randomized pizza recommendation (varying calorie/vegetarian/topping
   restrictions per iteration).
5. **GET `/api/ratings`** – Browse existing ratings.

Ramps from 0 to 20 virtual users over the ramp-up period, then holds for the test duration.

### 2. Register & Rate (`register_and_rate`)

Simulates the full account lifecycle with a smaller, constant number of virtual users:

1. **POST `/api/csrf-token`** – Fetch a CSRF token (required for login).
2. **POST `/api/users`** – Register a unique account.
3. **POST `/api/users/token/login`** – Log in and obtain an auth token.
4. **POST `/api/pizza`** – Get a pizza recommendation as the logged-in user.
5. **POST `/api/ratings`** – Rate a pizza.
6. **GET `/api/ratings`** – View the user's own ratings.

> [!NOTE]
> The pizza `id` returned by `POST /api/pizza` is not present in QuickPizza's ratable catalog on the
> public demo, so submitting a rating for it consistently returns `400 pizza ID not found`. To keep
> this scenario reliable, ratings are submitted against a known-good seeded pizza id (`1`-`50`)
> instead of chaining the freshly recommended id.
>
> There is also no cleanup step for created ratings: `DELETE /api/ratings` is rejected on the public
> demo with `403 operation not permitted for default user` - Grafana deliberately blocks bulk-deleting
> ratings there to protect the shared data set. Registered test users are otherwise harmless and are
> never reused.

Each request asserts HTTP status and expected response content, and the run enforces an overall
response-time SLA via thresholds.

---

## ⚙️ Load Scenario: 20 Virtual Users / 1 Minute

* **Browse & Recommend Threads (`USERS`)**: 20 (ramping)
* **Register & Rate Threads (`REGISTRATION_USERS`)**: 5 (constant)
* **Ramp-Up Period (`RAMPUP`)**: 60 seconds
* **Duration (`DURATION`)**: 60 seconds
* **Think time**: 0.5-3s randomized delay between steps, to emulate real browsing pace.

---

## ⚙️ Parameterization (Environment Variables)

| Variable | Description | Default |
| :--- | :--- | :--- |
| `BASE_URL` | Target host | `https://quickpizza.grafana.com` |
| `USERS` | Peak virtual users for the browse/recommend scenario | `20` |
| `REGISTRATION_USERS` | Constant virtual users for the register/rate scenario | `5` |
| `RAMPUP` | Time to ramp up to peak users (e.g. `60s`) | `60s` |
| `DURATION` | Total test execution time per scenario (e.g. `60s`) | `60s` |
| `DEMO_TOKEN` | Shared demo auth token for the browse/recommend scenario | `abcdef0123456789` |

---

## 🚀 Running the Tests

```bash
k6 run k6_performance_test.js
```

### Custom load parameters

```bash
k6 run -e USERS=20 -e RAMPUP=60s -e DURATION=60s -e REGISTRATION_USERS=5 k6_performance_test.js
```

### Smoke test (CI)

```bash
k6 run -e USERS=2 -e RAMPUP=2s -e DURATION=10s -e REGISTRATION_USERS=1 k6_performance_test.js
```

> [!NOTE]
> QuickPizza's public demo is shared infrastructure used by the whole k6 community for
> load-testing practice. Be considerate of load level and avoid running much higher concurrency
> against it than the 20-user scenario here.

---

## 🎯 Success Criteria (SLA)

1. **Error Rate**: < `5%`
2. **Average Response Time**: < `2000 ms`
3. **95th Percentile Response Time**: < `3000 ms`
4. **99th Percentile Response Time**: < `5000 ms`
