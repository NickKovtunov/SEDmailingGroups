<template>
  <div class="treeWrap">
    <div class="clearfix searchMenu">
      <div :class="{ currentSearchMenu: isUsualSearch, usualSearch: true }" @click="changeCurrentSearchMenu()">Поиск</div>
      <div :class="{ currentSearchMenu: !isUsualSearch, extendedSearch: true }" @click="changeCurrentSearchMenu()">Расширенный поиск</div>
    </div>

    <template v-if="isUsualSearch">
      <v-text-field v-model.trim="search"
                    placeholder="Наименование/Описание (минимум 3 символа)"
                    outlined
                    clearable
                    dense
                    @input="changeUsualSearch()"
                    autocomplete="off"></v-text-field>
    </template>
    <template v-else>
      <v-text-field v-model.trim="searchTitle"
                    placeholder="Наименование/Описание"
                    outlined
                    clearable
                    dense="dense"
                    autocomplete="off"></v-text-field>
      <div style="position: relative">
        <div class="chips">
          <span v-if="departments.length == 0" style="color: #9e9e9e;font-weight: 300;">Подразделение</span>
          <v-chip v-for="(d, index) in departments"
                  close
                  @click:close="departments = []">
            {{d.fullTitle}}
          </v-chip>
        </div>
        <v-icon @click.stop="openDepartmentsModel()" class="chip-edit">mdi-pencil</v-icon>

      </div>

      <v-autocomplete v-model="selectedUsers"
                      :items="usersRequestResult"
                      :loading="isLoadingUsers"
                      :search-input.sync="searchUsers"
                      outlined
                      dense
                      hide-no-data
                      item-text="fio"
                      item-value="userGuid"
                      chips
                      autocomplete="off"
                      background-color="white"
                      placeholder="Пользователи">
        <template v-slot:selection="data">
          <v-chip v-bind="data.attrs"
                  :input-value="data.selected"
                  close
                  @click="data.select"
                  @click:close="selectedUsers = null">
            {{ data.item.fio }}
          </v-chip>
        </template>
        <template v-slot:item="data">
          <v-list-item-content>
            <v-list-item-title v-html="data.item.fio"></v-list-item-title>
            <v-list-item-subtitle>({{data.item.department}}, {{data.item.position}})</v-list-item-subtitle>
          </v-list-item-content>
        </template>
      </v-autocomplete>

      <v-autocomplete v-model="selectedApproves"
                      :items="approvesRequestResult"
                      :loading="isLoadingApproves"
                      :search-input.sync="searchApproves"
                      outlined
                      dense
                      hide-no-data
                      item-text="fio"
                      item-value="userGuid"
                      chips
                      autocomplete="off"
                      background-color="white"
                      placeholder="Утверждающие">
        <template v-slot:selection="data">
          <v-chip v-bind="data.attrs"
                  :input-value="data.selected"
                  close
                  @click="data.select"
                  @click:close="selectedApproves = null">
            {{ data.item.fio }}
          </v-chip>
        </template>
        <template v-slot:item="data">
          <v-list-item-content>
            <v-list-item-title v-html="data.item.fio"></v-list-item-title>
            <v-list-item-subtitle>({{data.item.department}}, {{data.item.position}})</v-list-item-subtitle>
          </v-list-item-content>
        </template>
      </v-autocomplete>

      <v-autocomplete v-model="selectedAdmins"
                      :items="adminsRequestResult"
                      :loading="isLoadingAdmins"
                      :search-input.sync="searchAdmins"
                      outlined
                      dense
                      hide-no-data
                      item-text="fio"
                      item-value="userGuid"
                      chips
                      autocomplete="off"
                      background-color="white"
                      placeholder="Администраторы">
        <template v-slot:selection="data">
          <v-chip v-bind="data.attrs"
                  :input-value="data.selected"
                  close
                  @click="data.select"
                  @click:close="selectedAdmins = null">
            {{ data.item.fio }}
          </v-chip>
        </template>
        <template v-slot:item="data">
          <v-list-item-content>
            <v-list-item-title v-html="data.item.fio"></v-list-item-title>
            <v-list-item-subtitle>({{data.item.department}}, {{data.item.position}})</v-list-item-subtitle>
          </v-list-item-content>
        </template>
      </v-autocomplete>

      <v-autocomplete v-model="selectedExceptions"
                      :items="exceptionsRequestResult"
                      :loading="isLoadingExceptions"
                      :search-input.sync="searchExceptions"
                      outlined
                      dense
                      hide-no-data
                      item-text="fio"
                      item-value="userGuid"
                      chips
                      autocomplete="off"
                      background-color="white"
                      placeholder="Исключения">
        <template v-slot:selection="data">
          <v-chip v-bind="data.attrs"
                  :input-value="data.selected"
                  close
                  @click="data.select"
                  @click:close="selectedExceptions = null">
            {{ data.item.fio }}
          </v-chip>
        </template>
        <template v-slot:item="data">
          <v-list-item-content>
            <v-list-item-title v-html="data.item.fio"></v-list-item-title>
            <v-list-item-subtitle>({{data.item.department}}, {{data.item.position}})</v-list-item-subtitle>
          </v-list-item-content>
        </template>
      </v-autocomplete>

      <div class="clearfix">
        <v-btn style="float:right;" @click="extendedSearch">Найти</v-btn>
      </div>
    </template>

    <div v-if="!parents" class="text-center">
      <h1><icon icon="spinner" pulse /></h1>
    </div>

    <v-treeview v-else-if="parents.length > 0"
                :active.sync="active"
                :items="parents"
                :load-children="getChildren"
                :open.sync="open"
                dense
                activatable
                transition
                hoverable
                item-disabled="locked">
      <template v-slot:prepend="{ item, active }">
        <v-icon v-if="item.type == 1">mdi-folder-outline</v-icon>
      </template>
      <template slot="label"
                slot-scope="{ item }">
        <span :class="{ deletedNode: item.deleted == 'True', isSearch: item.isSearch }">{{ item.name }}</span>
      </template>
    </v-treeview>
    <span v-else>
      К сожалению, в базе отсутствуют данные, соответствующие Вашему запросу
    </span>

    <v-dialog v-model="departmentsDialog"
              scrollable
              max-width="700">
      <v-card>
        <v-card-title>
          Подразделения
        </v-card-title>

        <v-card-text style="height: 500px;">
          <app-departmentstree :treeReloadProperty="departmentTreeReloadProperty" @changeNode="changeDepartmentsNode($event)"></app-departmentstree>
        </v-card-text>

        <v-card-actions>
          <v-spacer></v-spacer>

          <v-btn color="blue darken-1"
                 text
                 @click="departmentsDialog = false">
            Закрыть
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
  export default {
    props: {
      showDeletedItems: {
        default: false,
        type: Boolean
      },
      treeReloadProperty: {
        default: false,
        type: Boolean
      },
      isMain: {
        default: false,
        type: Boolean
      },
      hiddenItem: {
        default: '',
        type: String
      },
    },

    data: () => ({
      active: [],
      open: [],
      parents: null,
      isUsualSearch: true,
      search: '',
      cancelSource: null,
      searchTitle: '',

      departmentsDialog: false,
      departments: [],
      departmentsResult: [],
      departmentTreeReloadProperty: false,

      usersRequestResult: [],
      isLoadingUsers: false,
      selectedUsers: null,
      searchUsers: null,

      approvesRequestResult: [],
      isLoadingApproves: false,
      selectedApproves: null,
      searchApproves: null,

        adminsRequestResult: [],
        isLoadingAdmins: false,
        selectedAdmins: null,
        searchAdmins: null,

      exceptionsRequestResult: [],
      isLoadingExceptions: false,
      selectedExceptions: null,
      searchExceptions: null,
    }),

    methods: {
      async loadPage() {
        this.getParents()
      },

      async getParents() {
        try {
          this.parents = null
          this.open = []
          this.active = []
          let response = await this.$http.get(this.$store.state.baseUrl + `/api/tree/getParents/?di=` + this.showDeletedItems + `&hi=` + this.hiddenItem + `&datetime=` + new Date().getMilliseconds())
          this.parents = response.data
        } catch (err) {
          window.alert("Ошибка загрузки родительских элементов")
          console.log(err)
        }
      },

      async getChildren(item) {
        try {
          return fetch(this.$store.state.baseUrl + `/api/tree/getChildren/?pid=` + item.id + `&di=` + this.showDeletedItems + `&hi=` + this.hiddenItem)
            .then(res => res.json())
            .then(json => (item.children.push(...json)))
            .catch(err => console.warn(err))
        } catch (err) {
          window.alert("Ошибка загрузки дочерних элементов")
          console.log(err)
        }
      },

      getSearchTree(searchString) {
        this.parents = null;
        this.open = []
        this.active = []

        this.cancelSearch();
        this.cancelSource = this.$http.CancelToken.source();

        this.$http.get(this.$store.state.baseUrl + `/api/tree/getSearchTree/?searchString=` + encodeURI(searchString) + `&di=` + this.showDeletedItems + `&hi=` + this.hiddenItem, {
          cancelToken: this.cancelSource.token
        }).then((response) => {
          this.parents = response.data.tree
          this.open = response.data.openNodes
          this.cancelSource = null
        }).catch(function (error) {
          console.log(error);
        });
      },

      extendedSearch() {
        this.parents = null;
        this.open = []
        this.active = []

        if (this.selectedUsers == null) {
          this.selectedUsers = ""
        }

        if (this.selectedApproves == null) {
          this.selectedApproves = ""
        }

        if (this.selectedAdmins == null) {
          this.selectedAdmins = ""
        }

        if (this.selectedExceptions == null) {
          this.selectedExceptions = ""
        }

        var departmentId = ""

        if (this.departments.length > 0) {
          departmentId = this.departments[0].id
        }

        this.$http.get(this.$store.state.baseUrl + `/api/tree/getSearchTree/?searchString=` + encodeURI(this.searchTitle) + `&di=` + this.showDeletedItems + `&hi=` + this.hiddenItem + `&user=` + this.selectedUsers + `&approves=` + this.selectedApproves + `&admins=` + this.selectedAdmins + `&exceptions=` + this.selectedExceptions + `&department=` + departmentId)
          .then((response) => {
            this.parents = response.data.tree
            this.open = response.data.openNodes
            this.cancelSource = null
          }).catch(function (error) {
            console.log(error);
          });
      },

      cancelSearch() {
        if (this.cancelSource) {
          this.cancelSource.cancel();
          console.log('cancel request done');
        }
      },

      changeUsualSearch() {
        if (this.search && this.search.length > 2) {
          this.getSearchTree(this.search, null)
        } else if (!this.search || this.search.length == 0) {
          this.getParents()
        }
      },

      changeCurrentSearchMenu() {
        this.isUsualSearch = !this.isUsualSearch;
        this.getParents()
      },

      getSearchTreeById() {
        this.parents = null;
        this.open = []
        this.active = []

        this.$http.get(this.$store.state.baseUrl + `/api/tree/getSearchTreeById/?id=` + this.$route.params.id + `&di=true`)
          .then((response) => {
            this.parents = response.data.tree
            this.open = response.data.openNodes
            this.active = [this.$route.params.id]
          }).catch(function (error) {
            console.log(error);
          });
      },

      reloadTree(changeDeleted = false) {
          if ((this.isMain) && ((this.$route.path.indexOf("show") != -1) || (this.$route.path.indexOf("edit") != -1))) {
            if (this.$route.params.id != undefined) {
              //var a = this.active.indexOf(this.$route.params.id) == -1
              if ((this.active.indexOf(this.$route.params.id) == -1) || (changeDeleted)) {
                this.getSearchTreeById()
              }
            }
        } else {
          this.loadPage()
        }
      },

      openDepartmentsModel() {
        this.departmentsDialog = true
      },

      async changeDepartmentsNode(id) {
        if (id.length > 0) {
          this.departmentsDialog = false
          let d = []
          d.id = id
          let response = await this.$http.get(this.$store.state.baseUrl + `/api/departmentsTree/GetFullDepartmentName/?id=` + d.id)
          d.fullTitle = response.data
          this.departments = []
          this.departments.push(d)
          this.departmentTreeReloadProperty = !this.departmentTreeReloadProperty
        } else {}
      },
    },

    created() {
      this.reloadTree()
    },

    watch: {
      showDeletedItems: function () {
        this.reloadTree(true)
      },
      treeReloadProperty: function () {
        this.reloadTree(true)
      },
      active: function () {
        this.$emit('changeNode', this.active)
      },
      $route(toR, fromR) {
        this.reloadTree()
      },

      selectedUsers() {
        this.searchUsers = null
      },

      selectedApproves() {
        this.searchApproves = null
      },

      selectedAdmins() {
        this.searchAdmins = null
      },

      selectedExceptions() {
        this.searchExceptions = null
      },

      searchUsers(val) {
        if (!val) {
          return
        }
        val = encodeURI(val)

        //if (this.isLoadingUsers) return

        this.isLoadingUsers = true
        fetch(this.$store.state.baseUrl + `/api/ADUser/Search?term=` + val)

          .then(res => res.json())
          .then(res => {
            this.usersRequestResult = this.usersRequestResult.concat(res)
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => (this.isLoadingUsers = false))
      },

      searchApproves(val) {
        if (!val) {
          return
        }
        val = encodeURI(val)

        //if (this.isLoadingApproves) return

        this.isLoadingApproves = true
        fetch(this.$store.state.baseUrl + `/api/ADUser/Search?term=` + val)
          
          .then(res => res.json())
          .then(res => {
            this.approvesRequestResult = this.approvesRequestResult.concat(res) 
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => (this.isLoadingApproves = false))
      },

      searchAdmins(val) {
        if (!val) {
          return
        }
        val = encodeURI(val)

        //if (this.isLoadingAdmins) return

        this.isLoadingAdmins = true
        fetch(this.$store.state.baseUrl + `/api/ADUser/Search?term=` + val)
          
          .then(res => res.json())
          .then(res => {
            this.adminsRequestResult = this.adminsRequestResult.concat(res) 
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => (this.isLoadingAdmins = false))
      },

      searchExceptions(val) {
        if (!val) {
          return
        }
        val = encodeURI(val)

        //if (this.isLoadingExceptions) return

        this.isLoadingExceptions = true
        fetch(this.$store.state.baseUrl + `/api/ADUser/Search?term=` + val)
          
          .then(res => res.json())
          .then(res => {
            this.exceptionsRequestResult = this.exceptionsRequestResult.concat(res) 
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => (this.isLoadingExceptions = false))
      },
    },
  }
</script>
