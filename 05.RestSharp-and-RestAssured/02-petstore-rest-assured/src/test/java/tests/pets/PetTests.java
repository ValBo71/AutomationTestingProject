package tests.pets;

import base.BaseApiTest;
import helpers.RandomDataGenerator;
import io.qameta.allure.*;
import io.restassured.response.Response;
import models.requests.PetRequest;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import java.util.Collections;

import static org.junit.jupiter.api.Assertions.assertEquals;

@Epic("API Tests")
@Feature("Pet Operations")
public class PetTests extends BaseApiTest {
    private PetRequest petPayload;
    private Long petId;

    @BeforeEach
    void setupPetData() {
        petId = RandomDataGenerator.generateId();
        petPayload = new PetRequest(
                petId,
                new PetRequest.Category(1, "Dogs"),
                "Buddy_" + petId,
                Collections.singletonList("https://example.com/photos/dog.jpg"),
                Collections.singletonList(new PetRequest.Tag(1, "friendly")),
                "available"
        );
    }

    @AfterEach
    void cleanupPet() {
        // Best-effort: remove the pet created in this test from the shared Swagger Petstore sandbox.
        // Safe to call even if the test already deleted it or never actually created it.
        try {
            petApiClient.deletePet(petId);
        } catch (Exception ignored) {
            // Nothing to clean up, or the sandbox is unreachable - don't fail the test on cleanup.
        }
    }

    @Test
    @DisplayName("Create pet with valid data should return 200")
    @Story("Create Pet")
    @Severity(SeverityLevel.CRITICAL)
    void createPet_withValidData_shouldReturnSuccess() {
        Response response = petApiClient.createPet(petPayload);
        assertEquals(200, response.statusCode());
        assertEquals(petId, response.jsonPath().getLong("id"));
    }

    @Test
    @DisplayName("Get pet by ID should return correct pet details")
    @Story("Get Pet")
    @Severity(SeverityLevel.NORMAL)
    void getPetById_withValidId_shouldReturnPetDetails() {
        petApiClient.createPet(petPayload);

        Response response = petApiClient.getPet(petId);
        assertEquals(200, response.statusCode());
        assertEquals(petId, response.jsonPath().getLong("id"));
        assertEquals("Buddy_" + petId, response.jsonPath().getString("name"));
    }

    @Test
    @DisplayName("Update pet name and status should persist changes")
    @Story("Update Pet")
    @Severity(SeverityLevel.NORMAL)
    void updatePet_withNewDetails_shouldPersistChanges() {
        petApiClient.createPet(petPayload);

        petPayload.setName("Buddy_Updated_" + petId);
        petPayload.setStatus("pending");

        Response response = petApiClient.updatePet(petPayload);
        assertEquals(200, response.statusCode());

        Response verifiedResponse = petApiClient.getPet(petId);
        assertEquals("Buddy_Updated_" + petId, verifiedResponse.jsonPath().getString("name"));
        assertEquals("pending", verifiedResponse.jsonPath().getString("status"));
    }

    @Test
    @DisplayName("Delete pet by ID should return 200")
    @Story("Delete Pet")
    @Severity(SeverityLevel.CRITICAL)
    void deletePet_withValidId_shouldReturnSuccess() {
        petApiClient.createPet(petPayload);

        Response response = petApiClient.deletePet(petId);
        assertEquals(200, response.statusCode());
    }
}
