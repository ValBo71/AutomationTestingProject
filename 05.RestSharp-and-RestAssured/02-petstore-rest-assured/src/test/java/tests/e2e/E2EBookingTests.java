package tests.e2e;

import base.BaseApiTest;
import helpers.RandomDataGenerator;
import helpers.RuntimeVariables;
import io.qameta.allure.*;
import io.restassured.response.Response;
import models.requests.OrderRequest;
import models.requests.PetRequest;
import models.requests.UserRequest;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Order;
import org.junit.jupiter.api.TestMethodOrder;
import org.junit.jupiter.api.MethodOrderer.OrderAnnotation;
import org.junit.jupiter.api.Test;
import java.util.Collections;

import static org.junit.jupiter.api.Assertions.assertEquals;

@Epic("API Tests")
@Feature("E2E Integration Flows")
@DisplayName("E2E booking and user lifecycle flow")
@TestMethodOrder(OrderAnnotation.class)
public class E2EBookingTests extends BaseApiTest {

    @BeforeAll
    public static void prepareE2EVariables() {
        // Equivalent to Postman Pre-request scripts generating unique values
        Long uniquePetId = RandomDataGenerator.generateId();
        String uniqueUsername = RandomDataGenerator.generateUsername();
        Long uniqueOrderId = RandomDataGenerator.generateId() - 50000;

        RuntimeVariables.set("petId", uniquePetId);
        RuntimeVariables.set("petName", "E2E_Buddy_" + uniquePetId);
        RuntimeVariables.set("photoUrl", "https://example.com/photos/e2e_" + uniquePetId + ".jpg");
        RuntimeVariables.set("username", uniqueUsername);
        RuntimeVariables.set("e2eEmail", uniqueUsername + "@example.com");
        RuntimeVariables.set("e2ePhone", RandomDataGenerator.generatePhone());
        RuntimeVariables.set("orderId", uniqueOrderId);
    }

    @Test
    @Order(1)
    @DisplayName("E2E Step 1: Create user account")
    @Story("User Registration")
    void step1_createUser() {
        UserRequest payload = new UserRequest(
                RandomDataGenerator.generateId(),
                RuntimeVariables.getString("username"),
                "E2E",
                "Customer",
                RuntimeVariables.getString("e2eEmail"),
                "SecretPassword123",
                RuntimeVariables.getString("e2ePhone"),
                0
        );

        Response response = userApiClient.createUser(payload);
        assertEquals(200, response.statusCode());
    }

    @Test
    @Order(2)
    @DisplayName("E2E Step 2: Login as user")
    @Story("User Authentication")
    void step2_loginUser() {
        Response response = userApiClient.loginUser(
                RuntimeVariables.getString("username"),
                "SecretPassword123"
        );
        assertEquals(200, response.statusCode());
    }

    @Test
    @Order(3)
    @DisplayName("E2E Step 3: Add pet to catalogue")
    @Story("Catalogue Additions")
    void step3_addPet() {
        PetRequest payload = new PetRequest(
                RuntimeVariables.getLong("petId"),
                new PetRequest.Category(2, "Dogs"),
                RuntimeVariables.getString("petName"),
                Collections.singletonList(RuntimeVariables.getString("photoUrl")),
                Collections.singletonList(new PetRequest.Tag(2, "friendly")),
                "available"
        );

        Response response = petApiClient.createPet(payload);
        assertEquals(200, response.statusCode());
    }

    @Test
    @Order(4)
    @DisplayName("E2E Step 4: Verify pet details exist")
    @Story("Catalogue Verification")
    void step4_verifyPet() {
        Response response = petApiClient.getPet(RuntimeVariables.getLong("petId"));
        assertEquals(200, response.statusCode());
        assertEquals(RuntimeVariables.getString("petName"), response.jsonPath().getString("name"));
    }

    @Test
    @Order(5)
    @DisplayName("E2E Step 5: Place purchase order for pet")
    @Story("Order Placements")
    void step5_placeOrder() {
        OrderRequest payload = new OrderRequest(
                RuntimeVariables.getLong("orderId"),
                RuntimeVariables.getLong("petId"),
                1,
                "2026-07-10T12:00:00.000Z",
                "placed",
                true
        );

        Response response = storeApiClient.placeOrder(payload);
        assertEquals(200, response.statusCode());
    }

    @Test
    @Order(6)
    @DisplayName("E2E Step 6: Verify order details match pet ID")
    @Story("Order Verifications")
    void step6_verifyOrder() {
        Response response = storeApiClient.getOrder(RuntimeVariables.getLong("orderId"));
        assertEquals(200, response.statusCode());
        assertEquals(RuntimeVariables.getLong("petId"), response.jsonPath().getLong("petId"));
    }

    @Test
    @Order(7)
    @DisplayName("E2E Step 7: Delete purchase order")
    @Story("Order Cleanups")
    void step7_deleteOrder() {
        Response response = storeApiClient.deleteOrder(RuntimeVariables.getLong("orderId"));
        assertEquals(200, response.statusCode());
    }

    @Test
    @Order(8)
    @DisplayName("E2E Step 8: Delete pet from catalogue")
    @Story("Catalogue Cleanups")
    void step8_deletePet() {
        Response response = petApiClient.deletePet(RuntimeVariables.getLong("petId"));
        assertEquals(200, response.statusCode());
    }

    @Test
    @Order(9)
    @DisplayName("E2E Step 9: Delete user account (Full Cleanup)")
    @Story("User Cleanups")
    void step9_deleteUser() {
        Response response = userApiClient.deleteUser(RuntimeVariables.getString("username"));
        assertEquals(200, response.statusCode());
    }
}
