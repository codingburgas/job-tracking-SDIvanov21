import { HttpInterceptorFn } from '@angular/common/http';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.includes('/api/Auth/login') || req.url.includes('/api/Auth/register')) {
    return next(req);
  }
  const token = localStorage.getItem('jwt');
  if (token) {
    const cloned = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
    return next(cloned);
  }
  return next(req);
}; 