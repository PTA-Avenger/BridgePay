import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Transaction, TransactionAnalytics, PagedResult, Merchant, Refund } from '../models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  // Transactions
  getTransactions(params?: {
    page?: number;
    pageSize?: number;
    status?: string;
    bank?: string;
    search?: string;
    dateFrom?: string;
    dateTo?: string;
  }): Observable<PagedResult<Transaction>> {
    let httpParams = new HttpParams();
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          httpParams = httpParams.set(key, String(value));
        }
      });
    }
    return this.http.get<PagedResult<Transaction>>(`${this.baseUrl}/transactions`, { params: httpParams });
  }

  getTransactionById(id: string): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.baseUrl}/transactions/${id}`);
  }

  submitTransaction(data: Partial<Transaction>): Observable<Transaction> {
    return this.http.post<Transaction>(`${this.baseUrl}/transactions`, data);
  }

  // Refunds
  getRefunds(): Observable<Refund[]> {
    return this.http.get<Refund[]>(`${this.baseUrl}/refunds`);
  }

  requestRefund(data: { transactionId: string; amount: number; reason: string }): Observable<Refund> {
    return this.http.post<Refund>(`${this.baseUrl}/refunds`, data);
  }

  // Merchant
  getMerchant(): Observable<Merchant> {
    return this.http.get<Merchant>(`${this.baseUrl}/merchant`);
  }

  updateMerchant(data: Partial<Merchant>): Observable<Merchant> {
    return this.http.put<Merchant>(`${this.baseUrl}/merchant`, data);
  }

  generateApiKey(merchantId: string): Observable<{ apiKey: string }> {
    return this.http.post<{ apiKey: string }>(`${this.baseUrl}/merchant/${merchantId}/api-key`, {});
  }

  // Analytics
  getAnalytics(dateFrom?: string, dateTo?: string): Observable<TransactionAnalytics> {
    let params = new HttpParams();
    if (dateFrom) params = params.set('dateFrom', dateFrom);
    if (dateTo) params = params.set('dateTo', dateTo);
    return this.http.get<TransactionAnalytics>(`${this.baseUrl}/analytics`, { params });
  }

  // Health
  getHealth(): Observable<{ status: string }> {
    return this.http.get<{ status: string }>(`${this.baseUrl}/health`);
  }
}
