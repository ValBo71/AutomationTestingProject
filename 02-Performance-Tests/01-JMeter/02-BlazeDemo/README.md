# Performance Testing Project for BlazeDemo

Load testing project for **BlazeDemo** (https://blazedemo.com/), BlazeMeter's mock travel agency demo site, built with **Apache JMeter**.

---

## 📂 Project Structure

1. **`BlazeDemo_Performance_Test.jmx`** – JMeter Test Plan with the flight search & booking scenario.
2. **`flight-routes.csv`** – From/To city pairs used to vary the search per virtual user.
3. **`README.md`** – This guide.

---

## ✈️ Scenario: Search and Book a Flight

A full end-to-end user journey through the site:

1. **GET `/`** – Load the home page.
2. **POST `/reserve.php`** – Search flights for a `fromPort`/`toPort` pair (from `flight-routes.csv`).
3. **POST `/purchase.php`** – Choose the first flight returned. The `flight`, `price`, and `airline` values are extracted dynamically from the reserve.php response via Regular Expression Extractors (not hardcoded), so the test stays correct even if BlazeDemo's flight data changes.
4. **POST `/confirmation.php`** – Submit the purchase form (traveler name, address, card details) and confirm the booking succeeded.

Each step asserts HTTP 200, checks for expected page content, and enforces a response-time SLA.

---

## ⚙️ Load Scenario: 20 Virtual Users / 1 Minute

* **Number of Threads (`users`)**: 20
* **Ramp-Up Period (`rampup`)**: 60 seconds
* **Duration (`duration`)**: 60 seconds
* **Think time**: 0.5–2s uniform random delay between steps, to emulate real browsing pace.

---

## ⚙️ Parameterization (JMeter Properties)

| Property | Description | Default |
| :--- | :--- | :--- |
| `users` | Number of parallel virtual users (Threads) | `20` |
| `rampup` | Time to spin up all virtual users (seconds) | `60` |
| `duration` | Total test execution time (seconds) | `60` |
| `loops` | Loop iterations per thread (`-1` = until duration ends) | `-1` |
| `baseUrl` | Target host | `blazedemo.com` |
| `protocol` | Target protocol | `https` |

---

## 🚀 Running the Tests

### GUI Mode (development/debugging)
1. Open Apache JMeter.
2. `File -> Open` → select `BlazeDemo_Performance_Test.jmx`.
3. Click **Start** and inspect results via `View Results Tree` / `Aggregate Report` / `Summary Report`.

### Non-GUI Mode (recommended for actual load runs)
```bash
jmeter -n -t BlazeDemo_Performance_Test.jmx -Jusers=20 -Jrampup=60 -Jduration=60 -l results_20_users.jtl -e -o report_20_users
```

> [!NOTE]
> BlazeDemo is a shared public demo site with no real backend persistence (bookings aren't
> actually charged or stored beyond the request). Still, be considerate of load level and
> avoid running much higher concurrency against it than the 20-user scenario here.

---

## 🎯 Success Criteria (SLA)

1. **Error Rate**: < `5%`
2. **Average Response Time**: < `2000 ms`
3. **95th Percentile Response Time**: < `3000 ms`
4. **99th Percentile Response Time**: < `5000 ms`
