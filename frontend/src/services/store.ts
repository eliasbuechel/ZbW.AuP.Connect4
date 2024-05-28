import { createStore } from "vuex";
import axios from "axios";

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
      try {
        const response = await axios.get("/account/checkAuthentication", {
          withCredentials: true,
        });
        commit("setAuthenticated", response.data.isAuthenticated);
      } catch (error) {
        commit("setAuthenticated", false);
      }
    },
  },
});
