import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpErrorResponse} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { RouterService } from './router.service';

@Injectable()
export class ErrorInterceptorService implements HttpInterceptor {

  constructor(private router: RouterService){}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<any> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 0) {
            this.router.routeToPath('server-down');
        }
        return throwError(error);
      })
    );
  }
}