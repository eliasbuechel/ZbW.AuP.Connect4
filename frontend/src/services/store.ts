import { createStore } from "vuex";
import axios, { AxiosResponse } from "axios";

export default createStore({
  state: {
    isAuthenticated: false,
  },
  mutations: {
    setAuthenticated(state, isAuthenticated) {
      state.isAuthenticated = isAuthenticated;
    },
  },
  actions: {
    async checkAuth({ commit }) {
      let response: AxiosResponse<any, any> | undefined = undefined;
      try {
        response = await axios.get("/account/checkAuthentication", {
          withCredentials: true,
        });
        commit("setAuthenticated", response!.data.isAuthenticated);
      } catch (error: any) {
        commit("setAuthenticated", false);
      }
    },
  },
});
