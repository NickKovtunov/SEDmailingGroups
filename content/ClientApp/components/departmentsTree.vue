<template>
  <div class="treeWrap">
    <v-text-field v-model.trim="search"
                  placeholder="Наименование/Описание (минимум 3 символа)"
                  outlined
                  clearable
                  dense
                  @input="changeUsualSearch()"></v-text-field>

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
                hoverable>
      <template slot="label"
                slot-scope="{ item }">
        <span :class="{ isSearch: item.isSearch }">{{ item.name }}</span>
      </template>
    </v-treeview>
    <span v-else>
      К сожалению, в базе отсутствуют данные, соответствующие Вашему запросу
    </span>
  </div>
</template>

<script>
  export default {
    props: {
      treeReloadProperty: {
        default: false,
        type: Boolean
      }
    },

    data: () => ({
      active: [],
      open: [],
      parents: null,
      search: '',
      cancelSource: null,
    }),

    watch: {
      treeReloadProperty: function () {
        this.loadPage()
      },
      active: function () {
        this.$emit('changeNode', this.active)
      },
    },

    methods: {
      async loadPage() {
        this.search = ""
        this.getParents()
      },

      async getParents() {
        try {
          this.parents = null
          this.open = []
          this.active = []
          let response = await this.$http.get(this.$store.state.baseUrl + `/api/departmentsTree/getParents/` + `?datetime=` + new Date().getMilliseconds())
          this.parents = response.data
        } catch (err) {
          window.alert("Ошибка загрузки родительских элементов")
          console.log(err)
        }
      },

      async getChildren(item) {
        try {
          return fetch(this.$store.state.baseUrl + `/api/departmentsTree/getChildren/?pid=` + item.id)
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

        this.$http.get(this.$store.state.baseUrl + `/api/departmentsTree/getSearchTree/?searchString=` + encodeURI(searchString), {
          cancelToken: this.cancelSource.token
        }).then((response) => {
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
    },

    created() {
      this.loadPage()
    },
  }
</script>
