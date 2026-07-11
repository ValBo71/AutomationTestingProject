package clients;

import models.requests.UserRequest;
import constants.ApiEndpoints;
import io.restassured.response.Response;

import java.util.Map;

public class UserApiClient extends ApiClient {
    public Response createUser(UserRequest body) {
        return postJson(ApiEndpoints.USER, body);
    }

    public Response getUser(String username) {
        return get(ApiEndpoints.USER_BY_USERNAME, "username", username);
    }

    public Response updateUser(String username, UserRequest body) {
        return putJson(ApiEndpoints.USER_BY_USERNAME, "username", username, body);
    }

    public Response deleteUser(String username) {
        return delete(ApiEndpoints.USER_BY_USERNAME, "username", username);
    }

    public Response loginUser(String username, String password) {
        return get(ApiEndpoints.USER_LOGIN, Map.of("username", username, "password", password));
    }

    public Response logoutUser() {
        return get(ApiEndpoints.USER_LOGOUT);
    }
}
