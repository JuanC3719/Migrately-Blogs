import React, { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import PropTypes from "prop-types";
import DOMPurify from "dompurify";
import "./blogsform.css";

const BlogsPreview = (props) => {
  _logger(props);
  const { state } = useLocation();
  const navigate = useNavigate();
  const [aBlog, setBlogArticle] = useState({
    id: "",
    authorId: "",
    blogType: "",
    title: "",
    content: "",
    imageUrl: "",
    subject: "",
    datePublish: "",
    isPublished: "",
  });

  useEffect(() => {
    if (state?.type === "BLOG_VIEW") {
      setBlogArticle((prevState) => {
        let newData = { ...prevState };
        newData = state.payload;
        _logger("new Data", newData);
        return newData;
      });
    }
  }, []);

  const onGoBack = (e) => {
    e.preventDefault();
    navigate("/blogs");
  };

  const clean = DOMPurify.sanitize(aBlog.content);

  const cleanPreview = DOMPurify.sanitize(props.blog?.content);

  const blogTypePreview = props.blogTypes?.filter(
    (type) => type.id === parseInt(props.blog.blogTypeId)
  );

  return (
    <React.Fragment>
      {state?.type === "BLOG_VIEW" ? (
        <>
          <div className="text-center mb-5"></div>
          <div className="row py-12 bg-dark landing-bg-image">
            <div className="d-flex justify-content-evenly">
              <div className="d-flex col-xl-6 col-md-8 col-12">
                <div className="card-body">
                  <div className="p-lg-6 card">
                    <div className="row">
                      <h1 className="card-title mb-3 col-12 text-center">
                        {aBlog.title}
                      </h1>
                      <h5 className="text-muted">
                        Topic: {aBlog.blogType.name}
                      </h5>
                      <img
                        className="rounded-top-md img-fluid"
                        src={aBlog.imageUrl}
                        alt=""
                      ></img>
                      <h3 className="card-text">{aBlog.subject}</h3>
                      <div
                        className="card-text"
                        dangerouslySetInnerHTML={{ __html: clean }}
                      ></div>
                      <button type="submit" className="btn btn-outline-primary">
                        Back to Blogs
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </>
      ) : (
        <div className="card-body">
          <div className="p-lg-6 card blogform-card-shadow">
            <div className="row">
              <h1 className="card-title mb-3 col-12 text-center">
                {props.blog?.title}
              </h1>
              <h5 className="text-muted">Topic: {blogTypePreview[0]?.name}</h5>
              <img
                className="rounded-top-md img-fluid"
                src={props.blog?.imageUrl}
                alt=""
              ></img>
              <h3 className="card-text">{props.blog?.subject}</h3>
              <div
                className="card-text"
                dangerouslySetInnerHTML={{ __html: cleanPreview }}
              ></div>
              <button
                type="submit"
                className="btn btn-outline-primary"
                onClick={onGoBack}
              >
                Back to Blogs
              </button>
            </div>
          </div>
        </div>
      )}
    </React.Fragment>
  );
};
BlogsPreview.propTypes = {
  blog: PropTypes.shape({
    id: PropTypes.number,
    blogTypeId: PropTypes.number.isRequired,
    blogType: PropTypes.shape({
      id: PropTypes.number.isRequired,
      name: PropTypes.string.isRequired,
    }),
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
export default BlogsPreview;
