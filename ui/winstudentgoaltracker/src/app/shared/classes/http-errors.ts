import { HttpErrorResponse } from '@angular/common/http';

// *****************************************************************
// Maps an HttpErrorResponse to a user-friendly diagnostic message.
// Status 0 means the browser never received a response (API
// unreachable, DNS failure, CORS block, etc.). 5xx errors typically
// indicate a backend issue such as a database connection failure.
// *****************************************************************
export function describeHttpError(error: HttpErrorResponse): string {
    // Try to extract a message from the response body first.
    const serverMessage = error.error?.message ?? error.error?.Message;

    switch (error.status) {
        case 0:
            return 'Unable to reach the server. Check that the API is running and accessible.';
        case 400:
            return serverMessage ?? 'Bad request (400).';
        case 401:
            return serverMessage ?? 'Not authorized (401).';
        case 403:
            return serverMessage ?? 'Access denied (403).';
        case 404:
            return 'Endpoint not found (404). The API may be running a different version.';
        case 500:
            return 'Server error (500). The API encountered an internal failure (possibly a database issue).';
        case 503:
            return 'Server unavailable (503). The API may be starting up or overwhelmed.';
        default:
            return `Unexpected error (${error.status}).`;
    }
}
