import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResult } from '../classes/api-result';
import { ResponseResult } from '../classes/auth.models';
import { ReportPromptDto } from '../classes/report-prompt.dto';
import { describeHttpError } from '../classes/http-errors';

@Injectable({
    providedIn: 'root',
})
export class ReportPromptService {

    // ************************** Declarations *************************

    private readonly http = inject(HttpClient);
    private readonly base = environment.apiBaseUrl;

    // ************************ Public Methods *************************

    // *****************************************************************
    // Returns the report prompt for the given reportname, scoped to
    // the authenticated user's program.
    // *****************************************************************
    async getByReportname(name: string): Promise<ApiResult<ReportPromptDto>> {
        try {
            const result = await firstValueFrom(
                this.http.get<ResponseResult<ReportPromptDto>>(
                    `${this.base}/api/ReportPrompt/by-name/${encodeURIComponent(name)}`
                )
            );
            return result.success && result.data
                ? ApiResult.ok(result.data)
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }

    // *****************************************************************
    // Updates the prompt text for an existing report prompt.
    // *****************************************************************
    async updatePrompt(id: string, prompt: string): Promise<ApiResult> {
        try {
            const result = await firstValueFrom(
                this.http.put<ResponseResult<void>>(
                    `${this.base}/api/ReportPrompt/${id}`,
                    { prompt }
                )
            );
            return result.success
                ? ApiResult.empty()
                : ApiResult.fail(result.message);
        } catch (error) {
            return ApiResult.fail(describeHttpError(error as HttpErrorResponse));
        }
    }
}
