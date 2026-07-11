# Performance Testing Project with Grafana k6

This project is a replication of the API performance and load testing project for [Automation Exercise](https://automationexercise.com) using **Grafana k6** — a modern, developer-centric open-source load testing tool.

---

## 📂 Project Structure

Within this project directory, you will find the following files:
1. **`k6_performance_test.js`**: The k6 load testing script written in JavaScript.
2. **`test-users.json`**: A JSON file containing user credentials for login verification.
3. **`README.md`**: This instruction and documentation guide.

---

## ⚙️ Load Scenarios & Parameterization

The k6 script contains two scenarios configured to run concurrently:
1. **`main_load`**: Simulates product listings, brand listings, product search, login, and user detail retrieval.
   * Parameterized VUs (`USERS`), Ramp-Up time (`RAMPUP`), and Test Duration (`DURATION`).
2. **`account_lifecycle`**: Simulates the full CRUD operations on user accounts with unique emails to test lifecycle performance under a low, constant load.
   * Parameterized concurrent VUs (`ACCOUNT_USERS`, default `2`).

---

## 🚀 How to Run the Tests

### Prerequisite: Install k6
You can install k6 using standard package managers:

* **Windows (using winget)**:
  ```bash
  winget install gnu.k6
  ```
* **macOS (using Homebrew)**:
  ```bash
  brew install k6
  ```
* **Linux (Debian/Ubuntu)**:
  ```bash
  sudo gpg -k
  sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD194422B70F34154F7AB0ABC1313ACCA50121
  echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | sudo tee /etc/apt/sources.list.d/k6.list
  sudo apt-get update
  sudo apt-get install k6
  ```

---

### 1. Dry Run / Smoke Test
Before running a full load test, perform a quick 1-iteration smoke test with 1 Virtual User (VU) to ensure there are no configuration or connection issues:
```bash
k6 run --vus 1 --iterations 1 k6_performance_test.js
```

### 2. Scenario 1: 20 Virtual Users / 1 Minute Load Test
To run a test with 20 parallel users ramping up over 60 seconds:
```bash
k6 run -e USERS=20 -e RAMPUP=60s -e DURATION=60s k6_performance_test.js
```

### 3. Scenario 2: 40 Virtual Users / 1 Minute Stress Test
To run a test with 40 parallel users ramping up over 60 seconds:
```bash
k6 run -e USERS=40 -e RAMPUP=60s -e DURATION=60s k6_performance_test.js
```

---

## 📊 Result Metrics & SLAs

k6 outputs detailed execution metrics directly to the console. The following metrics are monitored:

* **`http_req_failed`**: The rate of failed HTTP requests. Replicating our JMeter SLA, this must be **less than 5%** (`rate<0.05`).
* **`http_req_duration`**: The end-to-end request duration. We set explicit threshold checks per scenario:
  * **Average Response Time**: < `2000 ms` (`avg<2000`)
  * **95th Percentile Response Time**: < `3000 ms` (`p(95)<3000`)
  * **99th Percentile Response Time**: < `5000 ms` (`p(99)<5000`)
* **`vus`**: Active number of Virtual Users.
* **`iterations`**: Total number of scenario runs completed by all VUs.
* **`http_reqs`**: The throughput rate (total requests/second).
* **`data_received` / `data_sent`**: Data transfer volumes.

If any threshold SLA check is failed, k6 will exit with a non-zero status code, signalling a test failure (perfect for CI/CD integrations).
