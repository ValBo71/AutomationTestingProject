package clients;

import models.requests.UserRequest;
import constants.ApiEndpoints;
import io.restassured.response.Response;
import static io.restassured.RestAssured.given;

public class UserApiClient extends ApiClient {
    public Response createUser(UserRequest body) {
        return postJson(ApiEndpoints.USER, body);
    }

    public Response getUser(String username) {
        return get(ApiEndpoints.USER_BY_USERNAME, "username", username);
    }

    public Response updateUser(String username, UserRequest body) {
        return given()
                .spec(getRequestSpec())
                .pathParam("username", username)
                .body(body)
                .when()
                .put(ApiEndpoints.USER_BY_USERNAME)
                .then()
                .extract()
                .response();
    }

    public Response deleteUser(String username) {
        return delete(ApiEndpoints.USER_BY_USERNAME, "username", username);
    }

    public Response loginUser(String username, String password) {
        return given()
                .spec(getRequestSpec())
                .queryParam("username", username)
                .queryParam("password", password)
                .when()
                .get(ApiEndpoints.USER_LOGIN)
                .then()
                .extract()
                .response();
    }

    public Response logoutUser() {
        return get(ApiEndpoints.USER_LOGOUT);
    }
}
