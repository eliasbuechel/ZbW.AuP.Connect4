import axios from "axios";

class AuthService {
    public isAuthenticated(): boolean {
      const authToken = localStorage.getItem('authToken');
      return !!authToken;
    }

  
}
  
      
const authService = new AuthService()
export default authService;
  