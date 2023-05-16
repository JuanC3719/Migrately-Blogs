import React, { useEffect, useState, useRef } from "react";
import { useNavigate, useLocation, useParams } from "react-router-dom";
import PropTypes from "prop-types";
import { Button } from "react-bootstrap";
import "./blogsform.css";
import { blogAddSchema } from "schemas/blogAddSchema";
import blogsServices from "services/blogs/blogsService";
import lookUpService from "services/lookUpService";
import BlogsPreview from "./BlogsPreview";

import ClassicEditor from "@ckeditor/ckeditor5-build-classic";
import { CKEditor } from "@ckeditor/ckeditor5-react";
import { ErrorMessage, Field, Form, Formik } from "formik";
import Swal from "sweetalert2";
import toastr from "toastr";
import FileUpload from "components/fileUpload/FileUpload";


function BlogAddForm(props) {
  const currentUser = props.currentUser;
  const [blogTypes, setBlogTypes] = useState({ list: [], components: [] });
  const { state } = useLocation();
  const { id } = useParams();
  const ref = useRef(null);
  const navigate = useNavigate();
  const [editor, setEditor] = useState(false);

  const [formData, setFormData] = useState({
    blogTypeId: "",
    authorId: currentUser.id,
    title: "",
    subject: "",
    content: "",
    isPublished: false,
    imageUrl: "",
    datePublish: "",
  });

  useEffect(() => {
    if (!state) {
      blogsServices.getBlog(id).then(getBlogSuccess).catch(getBlogError);
    } else if (state?.type === "BLOG_EDIT") {
      setFormData(() => {
        const update = { ...formData, ...state.payload };

        return update;
      });
    }
  }, [props]);

  const getBlogSuccess = (response) => {
    const blogResponse = response.item;
    _logger("initial repsonse--->", response);
    _logger("response--->", response.item);

    toastr.success("Here's your Blog! Success retrieving blog", "Success");
    setFormData((prevState) => {
      const formData = { ...prevState, ...blogResponse };
      formData.blogTypeId = blogResponse.blogTypeId;
      formData.datePublished = response.item.datePublished;

      return formData;
    });
  };

  const getBlogError = (response) => {
    _logger("Error retrieving blog", response);

    toastr.error("Something's wrong! Error retrieving blog", "Error");
  };

  useEffect(() => {
    lookUpService
      .LookUp(["BlogTypes"])
      .then(onLookupSuccess)
      .catch(onLookupError);
  }, []);

  const onLookupSuccess = (response) => {
    setBlogTypes((prevState) => {
      const bt = { ...prevState };
      bt.list = response.item.blogTypes;
      bt.components = bt.list.map(mapBlogType);

      return bt;
    });
  };

  const onLookupError = (response) => {
    toastr.error("Something went wrong", "Error");
    _logger(response);
  };

  const mapBlogType = (blogType) => {
    return (
      <option key={blogType.id} value={blogType.id}>
        {blogType.name}
      </option>
    );
  };

  const handleSubmit = (values) => {
    Swal.fire(
      id
        ? {
            title: "Update Blog?",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Yes",
          }
        : {
            title: "Add Blog",
            text: "Would you like to post this blog?",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Add",
          }
    ).then((result) => {
      if (result["isConfirmed"]) {
        id
          ? blogsServices
              .updateBlog(id, values)
              .then(onUpdateSuccess)
              .catch(onUpdateError)
          : blogsServices.addBlog(values).then(onAddSuccess).catch(onAddError);
      } else if (result["isDenied"]) {
        Swal.fire("Changes are not saved", "", "info");
      }
    });
  };

  const onUpdateSuccess = (response) => {
    setFormData((prevstate) => {
      const prevBlog = { ...prevstate };
      _logger("update response--->", response);
      return prevBlog;
    });
    Swal.fire("Awesome!", "You updated the Blog!", "success");

    toastr.success("Your blog has been updated", "Success");

    if (currentUser?.roles.includes("Admin")) {
      navigate(`/blogs/${id}`);
    } else {
      navigate(`/blogs`);
    }
  };

  const onUpdateError = (response) => {
    toastr.error("Something's wrong! Can't Update", "Error");

    _logger(response);
  };

  const uploadFile = (file, setFieldValue) => {
    _logger(file);
    setFieldValue("imageUrl", file[0].url);
  };

  const onAddSuccess = (response) => {
    let successfulAddId = response.item;
    setFormData((prevState) => {
      const btp = { ...prevState };
      btp.id = successfulAddId;
      _logger("Blog Created Id", btp.id);

      return btp;
    });

    Swal.fire("Congrats!", "You wrote a Blog!", "success");

    toastr.success("Blog Submission Accepted", "Success");

    currentUser?.roles.includes("Admin")
      ? navigate(`/blogs`)
      : navigate(`/blogs`);
  };

  const onAddError = (response) => {
    Swal.fire({
      title: "Oops",
      text: "Something went wrong!",
      icon: "error",
    });
    toastr.error("Something's wrong! Can't submit due to error", "Error");
    currentUser?.roles.includes("Admin")
      ? navigate(`/blogs/add`)
      : navigate(`/`);
    _logger("OnAddError", response);
  };

  const onResetClick = async (resetForm) => {
    Swal.mixin({
      customClass: {
        confirmButton: "btn btn-success",
        cancelButton: "btn btn-danger",
      },
      buttonsStyling: false,
    });

    Swal.fire({
      title: "Are you sure you want to reset this form?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Yes, reset the form!",
      cancelButtonText: "No, back to draft!",
      reverseButtons: true,
    }).then(async (result) => {
      if (result.isConfirmed) {
        await resetForm();
        setEditor((prevState) => !prevState);
        Swal.fire("Draft Reset!", "Your draft has been reset.", "success");
      } else if (result.dismiss === Swal.DismissReason.cancel) {
        Swal.fire("Cancelled", "Your blog draft is safe :)");
      }
    });
  };

  return (
    <React.Fragment>
      <div className="container-fluid migrately-theme-background">
        <Formik
          innerRef={ref}
          enableReinitialize={true}
          initialValues={formData}
          onSubmit={handleSubmit}
          validationSchema={blogAddSchema}
          onReset={null}
        >
          {({ values, setFieldValue, resetForm }) => (
            <>
              <div className="text-center mb-5">
                <p className="lead flexbox display-2">Blog Submission Form</p>
              </div>

              <div className="row py-6 px-12 bg-white blogform-card-shadow">
                <div className="col-6">
                  <div className="card-body">
                    <div className="p-lg-6 card blogform-card-shadow">
                      {id ? (
                        <h2 className="card-title text-center">Edit Blog</h2>
                      ) : (
                        <h2 className="card-title text-center">Write a Blog</h2>
                      )}

                      <Form autoComplete="off">
                        <div className="row">
                          <div className="mb-3 col-12">
                            <div>
                              <label
                                htmlFor="blogTypeId"
                                className="form-label"
                              >
                                Blog Topic
                              </label>

                              <Field
                                as="select"
                                className="form-select"
                                id="blogTypeId"
                                placeholder="Category"
                                name="blogTypeId"
                                required
                              >
                                <option value="">Select</option>
                                {blogTypes.components}
                              </Field>

                              <ErrorMessage
                                name="blogTypeId"
                                component="div"
                                className="blogform-validation-error-message"
                              ></ErrorMessage>
                            </div>
                          </div>

                          <div className="mb-3 col-12">
                            <div>
                              <label htmlFor="title" className="form-label">
                                Blog Title
                              </label>

                              <Field
                                type="text"
                                className="form-control"
                                id="title"
                                placeholder="Title"
                                name="title"
                                required
                              ></Field>

                              <ErrorMessage
                                name="title"
                                component="div"
                                className="blogform-validation-error-message"
                              ></ErrorMessage>
                            </div>
                          </div>

                          <div className="mb-3 col-12">
                            <div>
                              <label htmlFor="subject" className="form-label">
                                Subject
                              </label>

                              <Field
                                type="text"
                                className="form-control"
                                id="subject"
                                placeholder="Subject"
                                name="subject"
                                required
                              ></Field>

                              <ErrorMessage
                                name="subject"
                                component="div"
                                className="blogform-validation-error-message"
                              ></ErrorMessage>
                            </div>
                          </div>

                          <div className="mb-3 col-12">
                            <div>
                              <label htmlFor="content" className="form-label">
                                Content
                              </label>

                              <div className="ck-editor__editable_inline">
                                {editor && (
                                  <CKEditor
                                    id="content"
                                    data={formData.content}
                                    name="content"
                                    editor={ClassicEditor}
                                    onChange={(event, editor) => {
                                      const content = editor?.getData();
                                      _logger(event);
                                      if (editor?.getData()) {
                                        setFieldValue("content", content);
                                      }
                                    }}
                                    required
                                  ></CKEditor>
                                )}
                                {!editor && (
                                  <CKEditor
                                    id="content"
                                    data={formData.content}
                                    name="content"
                                    editor={ClassicEditor}
                                    onChange={(event, editor) => {
                                      const content = editor?.getData();
                                      _logger(event);
                                      if (editor?.getData()) {
                                        setFieldValue("content", content);
                                      }
                                    }}
                                    required
                                  ></CKEditor>
                                )}
                              </div>
                            </div>
                          </div>

                          <div className="mb-3 col-12">
                            <div>
                              <label htmlFor="imageUrl" className="form-label">
                                Image Upload
                              </label>
                            </div>
                          </div>

                          <div className="mb-3 col-12">
                            <FileUpload
                              onUploadSuccess={(file) =>
                                uploadFile(file, setFieldValue)
                              }
                              name="imageUrl"
                            ></FileUpload>
                          </div>

                          <div className="mb-3 col-6 form-check form-switch">
                            <div>
                              <label
                                className="form-check-label"
                                name="isPublished"
                              >
                                Publish this Blog for display?
                              </label>

                              <Field
                                className="form-check-input"
                                type="checkbox"
                                name="isPublished"
                                role="switch"
                                id="isPublished"
                                required="error"
                              ></Field>
                            </div>
                          </div>

                          <div className="mb-3 col-6">
                            <div>
                              <label
                                htmlFor="datePublish"
                                className="form-label"
                              >
                                Published On
                              </label>

                              <Field
                                type="date"
                                className="form-control"
                                id="datePublish"
                                placeholder="datePublish"
                                name="datePublish"
                                required
                              ></Field>
                            </div>
                          </div>
                          <div className="col-6 btn-group">
                            <Button
                              id="submit"
                              type="submit"
                              variant="outline-primary"
                              className="mx-2"
                            >
                              Post
                            </Button>
                            <Button
                              onClick={() => onResetClick(resetForm)}
                              variant="outline-warning"
                            >
                              Reset
                            </Button>
                          </div>
                        </div>
                      </Form>
                    </div>
                  </div>
                </div>

                <div className="col-6">
                  {
                    <BlogsPreview
                      currentUser={currentUser.id}
                      blog={values}
                      blogTypes={blogTypes.list}
                    />
                  }
                </div>
              </div>
            </>
          )}
        </Formik>
      </div>
    </React.Fragment>
  );
}

BlogAddForm.propTypes = {
  blog: PropTypes.shape({
    id: PropTypes.number.isRequired,
    blogTypeId: PropTypes.number.isRequired,
    blogType: PropTypes.shape({
      id: PropTypes.number.isRequired,
      name: PropTypes.string.isRequired,
    }).isRequired,
    title: PropTypes.string.isRequired,
    content: PropTypes.string.isRequired,
    imageUrl: PropTypes.string,
    subject: PropTypes.string.isRequired,
    isPublished: PropTypes.bool.isRequired,
    publishedOn: PropTypes.instanceOf(Date).isRequired,
  }).isRequired,
  currentUser: PropTypes.shape({
    id: PropTypes.number.isRequired,
    isLoggedIn: PropTypes.bool.isRequired,
    roles: PropTypes.string.isRequired,
  }).isRequired,
  blogTypes: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    filter: PropTypes.func.isRequired,
  }).isRequired,
};

export default BlogAddForm;
