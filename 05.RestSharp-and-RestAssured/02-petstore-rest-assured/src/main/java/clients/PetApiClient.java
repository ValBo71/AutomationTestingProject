package clients;

import models.requests.PetRequest;
import constants.ApiEndpoints;
import io.restassured.response.Response;

public class PetApiClient extends ApiClient {
    public Response createPet(PetRequest body) {
        return postJson(ApiEndpoints.PET, body);
    }

    public Response getPet(Long petId) {
        return get(ApiEndpoints.PET_BY_ID, "petId", petId);
    }

    public Response updatePet(PetRequest body) {
        return putJson(ApiEndpoints.PET, body);
    }

    public Response deletePet(Long petId) {
        return delete(ApiEndpoints.PET_BY_ID, "petId", petId);
    }
}
