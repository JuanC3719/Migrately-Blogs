import axios from "axios";
import {
  API_HOST_PREFIX,
  onGlobalError,
  onGlobalSuccess,
} from "services/serviceHelpers";

const blogsService = {
  endpoint: `${API_HOST_PREFIX}/api/blogs`,
};

const getBlog = (id, payload) => {
  const config = {
    method: "GET",
    url: blogsService.endpoint + "/" + id,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const getBlogs = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: `${blogsService.endpoint}/paginate/?PageIndex=${pageIndex}&pageSize=${pageSize}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const updateBlog = (id, payload) => {
  const config = {
    method: "PUT",
    url: blogsService.endpoint + "/" + id,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const addBlog = (payload) => {
  const config = {
    method: "POST",
    url: blogsService.endpoint,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const getByBlogType = (pageIndex, pageSize, blogTypeId) => {
  const config = {
    method: "GET",
    url: `${blogsService.endpoint}/blogtypes/?pageIndex=${pageIndex}&pageSize=${pageSize}&blogTypeId=${blogTypeId}`,
    crossdomain: true,
    withCredentials: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const getBySearchBlogs = (pageIndex, pageSize, query) => {
  const config = {
    method: "GET",
    url:
      blogsService.endpoint +
      `/search/?PageIndex=${pageIndex}&pageSize=${pageSize}&query=${query}`,
    crossdomain: true,
    withCredentials: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};
const getBlogByAuthorId = (pageIndex, pageSize, authorId) => {
  const config = {
    method: "GET",
    url:
      blogsService.endpoint +
      `/authors?pageIndex=${pageIndex}&pageSize=${pageSize}&authorId=${authorId}`,
    crossdomain: true,
    withCredentials: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

const blogsServices = {
  addBlog,
  updateBlog,
  getBlogs,
  getBlog,
  getByBlogType,
  getBySearchBlogs,
  getBlogByAuthorId,
};

export default blogsServices;
