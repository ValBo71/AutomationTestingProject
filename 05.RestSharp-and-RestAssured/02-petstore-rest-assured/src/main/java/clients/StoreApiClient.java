package clients;

import models.requests.OrderRequest;
import constants.ApiEndpoints;
import io.restassured.response.Response;

public class StoreApiClient extends ApiClient {
    public Response placeOrder(OrderRequest body) {
        return postJson(ApiEndpoints.STORE_ORDER, body);
    }

    public Response getOrder(Long orderId) {
        return get(ApiEndpoints.STORE_ORDER_BY_ID, "orderId", orderId);
    }

    public Response deleteOrder(Long orderId) {
        return delete(ApiEndpoints.STORE_ORDER_BY_ID, "orderId", orderId);
    }

    public Response getInventory() {
        return get(ApiEndpoints.STORE_INVENTORY);
    }
}
