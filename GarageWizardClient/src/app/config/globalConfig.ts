
export class Configuration
{
    static garageApiURL : string = "http://localhost:5062/";
    static garageApiRegisterEndpoint = "account/register"; 
    static garageApiLoginEndpoint = "account/login";
    static garageApiLogoutEndpoint = "account/logout";
    static garageApiListVehiclesEndpoint = "vehicle/info";
    static garageApiListVehiclesNotOnRepairEndpoint = "vehicle/norepair/info";
    static garageApiGetSpecificVehicleEndpoint = "vehicle/specifics";
    static garageApiListJobsEndpoint = "job/info";
    static garageApiListRepairsEndpoint = "job/specifics"; // Query: vehicleID
    static garageApiCreateRepairsEndpoint = "job/assign"; // vehicleID, jobs
    static garageApiCompleteRepairsEndpoint = "job/specifics/complete";
    static garageApiListSpotsEndpoint = "spot/info";
    static garageApiListRolesEndpoint = "role/info";
    static garageApiParkVehicleEndpoint = "vehicle/park";
    static garageApiRemoveVehicleEndpoint = "vehicle/remove";
    static garageApiVacantSpotCheckEndpoint = "spot/reserve/check"; // [ Query params: vecID, wheels]
    static garageApiReserveSpotEndpoint = "spot/reserve"; 
    static garageApiCreateSpotEndpoint = "spot/create";
    static garageApiRemoveSpotEndpoint = "spot/remove";
    static garageApiEditRoleEndpoint = "role/edit";
    static garageApiEditRoleAccessEndpoint = "role/access/edit";
    static garageApiGetRoleDetailsEndpoint = "role/details"; // roleID
    static garageApiGetUsersEndpoint = "users/info";
    static garageApiGetCurrentUserEndpoint = "users/current";
    static garageApiChangeUserPasswordEndpoint = "users/current/change";
    static garageApiReleaseSpotEndpoint = "spot/release";
    static garageApiCreateJobEndpoint = "job/create";
    static garageApiRemoveJobEndpoint = "job/remove";
    static garageApiGetUploadedJobImage = "job/uploads/get";
    static garageApiUploadJobImage = "job/uploads/place";
    static garageApiStatsEndpoint = "stats";
}