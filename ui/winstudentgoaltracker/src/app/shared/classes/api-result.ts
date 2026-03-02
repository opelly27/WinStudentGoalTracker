// *****************************************************************
// Standard wrapper for API responses. On success, the payload
// contains the returned data. On failure, message contains the
// error description from the server.
// *****************************************************************
export class ApiResult<T = void> {
    success: boolean;
    payload: T | null;
    message: string;

    private constructor(success: boolean, payload: T | null, message: string) {
        this.success = success;
        this.payload = payload;
        this.message = message;
    }

    // *****************************************************************
    // Creates a successful result with the given payload.
    // *****************************************************************
    static ok<T>(payload: T): ApiResult<T> {
        return new ApiResult<T>(true, payload, '');
    }

    // *****************************************************************
    // Creates a successful result with no payload.
    // *****************************************************************
    static empty(): ApiResult<void> {
        return new ApiResult<void>(true, null, '');
    }

    // *****************************************************************
    // Creates a failed result with the given error message.
    // *****************************************************************
    static fail<T = void>(message: string): ApiResult<T> {
        return new ApiResult<T>(false, null, message);
    }
}
