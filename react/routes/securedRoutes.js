const AddBlog = lazy(() => import("../components/blogs/BlogForm"));

const blogRoutes = [
  {
    path: "blogs/add",
    name: "AddBlog",
    exact: true,
    element: AddBlog,
    roles: ["Admin", "User"],
    isAnonymous: true,
  },
  {
    path: "blogs/:id/edit",
    name: "EditBlog",
    exact: true,
    element: AddBlog,
    roles: ["Admin", "User"],
    isAnonymous: true,
  },
];

export default blogRoutes;
