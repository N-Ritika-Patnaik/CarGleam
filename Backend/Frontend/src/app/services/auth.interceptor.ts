import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('jwtToken');
  console.log('token', token);
  
  if (token) {
   const cloneReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    }); 
    console.log('Request with Authorization:', cloneReq);
    return next(cloneReq);
  } 
  return next(req);
};

 
 
  
 
 
 
 

 

 
 
