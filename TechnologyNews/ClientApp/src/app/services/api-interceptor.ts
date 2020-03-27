import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class APIInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    //https://localhost:44341/your-api-url/http://localhost/api/login
    let firstSlash = false;
    let newReqUrl = req.url.substr(req.url.indexOf("://") + 3);
    newReqUrl = newReqUrl.substr(newReqUrl.indexOf("/"))
    const apiReq = req.clone({ url: `${newReqUrl}`, setHeaders: { 'content-type': 'application/json' } });
    //your - api - url /
    return next.handle(apiReq);
  }
}
