package clients;

import config.ConfigReader;
import io.restassured.RestAssured;
import io.restassured.builder.RequestSpecBuilder;
import io.restassured.filter.log.RequestLoggingFilter;
import io.restassured.filter.log.ResponseLoggingFilter;
import io.restassured.http.ContentType;
import io.restassured.response.Response;
import io.restassured.specification.RequestSpecification;
import io.qameta.allure.restassured.AllureRestAssured;

import java.util.Map;

import static io.restassured.RestAssured.given;

public class ApiClient {
    protected static RequestSpecification getRequestSpec() {
        return new RequestSpecBuilder()
                .setBaseUri(ConfigReader.getBaseUrl())
                .setBasePath("/v2")
                .setContentType(ContentType.JSON)
                .setAccept(ContentType.JSON)
                .addFilter(new AllureRestAssured())
                .addFilter(new RequestLoggingFilter())
                .addFilter(new ResponseLoggingFilter())
                .build();
    }

    protected Response get(String endpoint) {
        return given()
                .spec(getRequestSpec())
                .when()
                .get(endpoint)
                .then()
                .extract()
                .response();
    }

    protected Response get(String endpoint, String paramName, Object paramValue) {
        return given()
                .spec(getRequestSpec())
                .pathParam(paramName, paramValue)
                .when()
                .get(endpoint)
                .then()
                .extract()
                .response();
    }

    protected Response postJson(String endpoint, Object body) {
        return given()
                .spec(getRequestSpec())
                .body(body)
                .when()
                .post(endpoint)
                .then()
                .extract()
                .response();
    }

    protected Response putJson(String endpoint, Object body) {
        return given()
                .spec(getRequestSpec())
                .body(body)
                .when()
                .put(endpoint)
                .then()
                .extract()
                .response();
    }

    protected Response putJson(String endpoint, String paramName, Object paramValue, Object body) {
        return given()
                .spec(getRequestSpec())
                .pathParam(paramName, paramValue)
                .body(body)
                .when()
                .put(endpoint)
                .then()
                .extract()
                .response();
    }

    protected Response get(String endpoint, Map<String, ?> queryParams) {
        return given()
                .spec(getRequestSpec())
                .queryParams(queryParams)
                .when()
                .get(endpoint)
                .then()
                .extract()
                .response();
    }

    protected Response delete(String endpoint, String paramName, Object paramValue) {
        return given()
                .spec(getRequestSpec())
                .pathParam(paramName, paramValue)
                .when()
                .delete(endpoint)
                .then()
                .extract()
                .response();
    }
}
