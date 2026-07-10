package tests.store;

import base.BaseApiTest;
import io.qameta.allure.*;
import io.restassured.response.Response;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

@Epic("API Tests")
@Feature("Store Operations")
public class StoreTests extends BaseApiTest {

    @Test
    @DisplayName("Get store inventory should return inventory map")
    @Story("Get Inventory")
    @Severity(SeverityLevel.NORMAL)
    void getInventory_shouldReturnInventoryMap() {
        Response response = storeApiClient.getInventory();
        assertEquals(200, response.statusCode());
        assertNotNull(response.jsonPath().getMap(""));
    }
}
