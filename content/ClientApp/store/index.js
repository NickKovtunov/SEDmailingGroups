import Vue from 'vue'
import router from '../router/index'
import Vuex from 'vuex'

Vue.use(Vuex)

// TYPES
//const MAIN_SET_COUNTER = 'MAIN_SET_COUNTER'

// STATE
const state = {
  baseUrl: document.location.href.slice(0, document.location.href.indexOf('#', 0)),
  treeReloadProperty: true,
  //hasRights: this.$http.get(this.$store.state.baseUrl + `/api/values/HasRights/`),
}

// MUTATIONS
const mutations = {
  //[MAIN_SET_COUNTER] (state, obj) {
  //  state.counter = obj.counter
  //}
}

// ACTIONS
const actions = ({
  //setCounter ({ commit }, obj) {
  //  commit(MAIN_SET_COUNTER, obj)
  //}
})

export default new Vuex.Store({
  state,
  mutations,
  actions
})
