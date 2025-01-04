### 2020-07-29
* **Nwpie.Foundation.Location.ServiceCore (1.3.1)**
  * config: Start use ConfigServer V2

### 2020-06-18
* **Nwpie.Foundation.Location.ServiceCore (1.3.0.2)**
  * docs: Modify location service mapping in xml files
  * fix: `ConfigKey` case sensitive issue between V1 and V2 Config Server

### 2020-05-30
* **Nwpie.Foundation.Location.ServiceCore (1.3.0)**
  * feat: Bridge configserver V1 and V2 routes with Controller Name = `configserver` and `config` respectively
  * BREAKING CHANGE: env CARK_BASE_SERVICE_URL to `SDK_CONFIG_SERVICE_URL`

### 2020-05-08
* **Nwpie.Foundation.Location.ServiceCore (1.2.0)**
  * fix: /configserver/get should give custom apiKey and apiName arguments

### 2020-03-16
* **Nwpie.Foundation.Location.Endpoint (1.1.0)**
  * chore: Replace /acct with `/auth` to check apikey

### 2020-01-21
* **Nwpie.Foundation.Location.Endpoint (1.0.0)**
  * feat: Proxy config server
* **Nwpie.Foundation.Location.Contract (1.0.0)**
