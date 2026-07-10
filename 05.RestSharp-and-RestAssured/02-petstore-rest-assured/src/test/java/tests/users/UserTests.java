package tests.users;

import base.BaseApiTest;
import helpers.RandomDataGenerator;
import io.qameta.allure.*;
import io.restassured.response.Response;
import models.requests.UserRequest;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

@Epic("API Tests")
@Feature("User Operations")
public class UserTests extends BaseApiTest {
    private UserRequest userPayload;
    private String username;

    @BeforeEach
    void setupUserData() {
        username = RandomDataGenerator.generateUsername();
        userPayload = new UserRequest(
                RandomDataGenerator.generateId(),
                username,
                "John",
                "Doe",
                RandomDataGenerator.generateEmail(),
                "pass123",
                RandomDataGenerator.generatePhone(),
                0
        );
    }

    @Test
    @DisplayName("Create user with valid data should return 200")
    @Story("Create User")
    @Severity(SeverityLevel.CRITICAL)
    void createUser_withValidData_shouldReturnSuccess() {
        Response response = userApiClient.createUser(userPayload);
        assertEquals(200, response.statusCode());
    }

    @Test
    @DisplayName("Get user by username should return correct user profile")
    @Story("Get User")
    @Severity(SeverityLevel.NORMAL)
    void getUserByUsername_withValidUsername_shouldReturnProfile() {
        userApiClient.createUser(userPayload);

        Response response = userApiClient.getUser(username);
        assertEquals(200, response.statusCode());
        assertEquals(username, response.jsonPath().getString("username"));
    }

    @Test
    @DisplayName("Update user profile should persist changes")
    @Story("Update User")
    @Severity(SeverityLevel.NORMAL)
    void updateUser_withNewDetails_shouldPersistChanges() {
        userApiClient.createUser(userPayload);

        userPayload.setFirstName("John_Updated");
        userPayload.setLastName("Doe_Updated");

        Response response = userApiClient.updateUser(username, userPayload);
        assertEquals(200, response.statusCode());

        Response verified = userApiClient.getUser(username);
        assertEquals("John_Updated", verified.jsonPath().getString("firstName"));
    }

    @Test
    @DisplayName("Delete user by username should return 200")
    @Story("Delete User")
    @Severity(SeverityLevel.CRITICAL)
    void deleteUser_withValidUsername_shouldReturnSuccess() {
        userApiClient.createUser(userPayload);

        Response response = userApiClient.deleteUser(username);
        assertEquals(200, response.statusCode());
    }
}
