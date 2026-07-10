# Performance Testing Project for Automation Exercise APIs

This project is designed for performance and load testing of the API endpoints provided by the **Automation Exercise** platform.

The project is built using **Apache JMeter** and supports dynamic parameterization via JMeter properties, allowing you to easily scale and adjust load scenarios directly from the command line.

---

## 📂 Project Structure

Within this project directory, you will find the following files:
1. **`AutomationExercise_Performance_Test.jmx`**: The main JMeter Test Plan configuration file containing HTTP samplers, assertions, timers, and listeners.
2. **`test-users.csv`**: A CSV file containing test user credentials for the login verification scenario.
3. **`README.md`**: This instruction and documentation guide.

---

## 🔗 Tested API Endpoints

In accordance with the official [Automation Exercise API List](https://automationexercise.com/api_list), the following endpoints are configured:

1. **Main Scenario (Main Load Test Thread Group):**
   * **GET** `/api/productsList` – Get all products list.
   * **GET** `/api/brandsList` – Get all brands list.
   * **POST** `/api/searchProduct` – Search product (defaults to search query: `tshirt`).
   * **POST** `/api/verifyLogin` – Verify login with credentials loaded from `test-users.csv`.
   * **GET** `/api/getUserDetailByEmail` – Get user account details by email.

2. **Account Lifecycle (Optional / Lower Load Thread Group):**
   * **POST** `/api/createAccount` – Create a new user account with a dynamically generated unique email address.
   * **GET** `/api/getUserDetailByEmail` – Verify that the account details can be retrieved successfully.
   * **PUT** `/api/updateAccount` – Update account details.
   * **DELETE** `/api/deleteAccount` – Delete the account to keep test data clean.

---

## ⚙️ Load Scenarios

The project supports two main load scenarios controlled dynamically via JMeter Properties:

### Scenario 1: 20 Virtual Users / 1 Minute
* **Number of Threads (`users`)**: 20
* **Ramp-Up Period (`rampup`)**: 60 seconds (threads are introduced gradually)
* **Duration (`duration`)**: 60 seconds
* **Objective**: Evaluate API behavior under standard concurrent load.

### Scenario 2: 40 Virtual Users / 1 Minute
* **Number of Threads (`users`)**: 40
* **Ramp-Up Period (`rampup`)**: 60 seconds
* **Duration (`duration`)**: 60 seconds
* **Objective**: Stress test APIs under double the standard concurrency.

---

## ⚙️ Parameterization (JMeter Properties)

The following parameters are exposed as JMeter Properties and can be customized at runtime:

| Property | Description | Default Value |
| :--- | :--- | :--- |
| `users` | Number of parallel virtual users (Threads) | `20` |
| `rampup` | Time to spin up all virtual users (seconds) | `60` |
| `duration` | Total test execution time (seconds) | `60` |
| `loops` | Number of loop iterations per thread (`-1` for infinite loops during duration) | `-1` |
| `searchProduct`| Product search query parameter in the search request | `tshirt` |
| `accountUsers` | Number of threads for the account lifecycle flow (lowered to prevent database lock/flooding) | `2` |

---

## 🚀 Running the Tests

### 1. GUI Mode (For Script Development & Debugging)
1. Open Apache JMeter.
2. Load the test plan: `File -> Open` -> Select `AutomationExercise_Performance_Test.jmx`.
3. Review request parameters, header assertions, and timers.
4. Click the green **Start** button to execute locally.
5. Inspect results in real-time using listeners: `View Results Tree`, `Aggregate Report`, and `Summary Report`.

### 2. Non-GUI Mode (Command Line - Recommended for Load Testing)
To get accurate results and conserve system resources, load tests should always be executed in command-line mode.

#### Run Scenario 1 (20 Users):
```bash
jmeter -n -t AutomationExercise_Performance_Test.jmx -Jusers=20 -Jrampup=60 -Jduration=60 -l results_20_users.jtl -e -o report_20_users
```

#### Run Scenario 2 (40 Users):
```bash
jmeter -n -t AutomationExercise_Performance_Test.jmx -Jusers=40 -Jrampup=60 -Jduration=60 -l results_40_users.jtl -e -o report_40_users
```

> [!NOTE]
> * `-n` tells JMeter to run in non-GUI mode.
> * `-t` specifies the path to the `.jmx` test plan.
> * `-J<property>=<value>` passes dynamic properties overriding defaults.
> * `-l` defines the file where raw test results (`.jtl` file) will be saved.
> * `-e -o <folder>` automatically generates a comprehensive dashboard report in the specified output directory once the test finishes.

---

## 📊 Result Metrics & Interpretation

The HTML report and JMeter listeners provide the following metrics:

* **Samples**: The total number of requests sent during the test run.
* **Average**: The mean response time of the requests (in milliseconds).
* **Min**: The fastest recorded response time from the server.
* **Max**: The slowest recorded response time.
* **Std. Dev. (Standard Deviation)**: Indicates response time variation. A lower standard deviation means more consistent response times.
* **Error %**: The percentage of failed requests relative to the total number of requests.
* **Throughput**: The capacity of the server, measured in requests processed per second (TPS). Higher is better.
* **Received KB/sec / Sent KB/sec**: Network data transfer throughput.
* **Percentiles (90th, 95th, 99th)**:
  * **95th Percentile = 3000 ms** indicates that 95% of all requests completed within 3000 ms, while the remaining 5% took longer. This is the main user experience metric.

---

## 🎯 Success Criteria (Performance Acceptance Criteria / SLA)

A load test is considered **successful** if all the following SLA thresholds are met:

1. **Error Rate**: < `5%`
2. **Average Response Time**: < `2000 ms`
3. **95th Percentile Response Time**: < `3000 ms`
4. **99th Percentile Response Time**: < `5000 ms`

If any of these metrics exceed the thresholds, it indicates a potential performance regression or bottleneck in the target system.
