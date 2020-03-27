"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var APIInterceptor = /** @class */ (function () {
    function APIInterceptor() {
    }
    APIInterceptor.prototype.intercept = function (req, next) {
        var apiReq = req.clone({ url: "your-api-url/" + req.url });
        return next.handle(apiReq);
    };
    return APIInterceptor;
}());
exports.APIInterceptor = APIInterceptor;
//# sourceMappingURL=api-interceptor.js.map