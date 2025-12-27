import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import "./Auth.css";

const Register = () => {
  const navigate = useNavigate();
  const { register } = useAuth();
  const [formData, setFormData] = useState({
    email: "",
    password: "",
    confirmPassword: "",
    fullName: "",
    phoneNumber: "",
    address: "",
    city: "",
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
    // Clear error for this field
    if (errors[e.target.name]) {
      setErrors({
        ...errors,
        [e.target.name]: "",
      });
    }
  };

  const validateForm = () => {
    const newErrors = {};

    if (!formData.email) {
      newErrors.email = "Email is required";
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = "Email is invalid";
    }

    if (!formData.password) {
      newErrors.password = "Password is required";
    } else if (formData.password.length < 8) {
      newErrors.password = "Password must be at least 8 characters";
    }

    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = "Passwords do not match";
    }

    if (!formData.fullName) {
      newErrors.fullName = "Full name is required";
    }

    if (!formData.phoneNumber) {
      newErrors.phoneNumber = "Phone number is required";
    }

    if (!formData.address) {
      newErrors.address = "Address is required";
    }

    if (!formData.city) {
      newErrors.city = "City is required";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setLoading(true);

    try {
      const { confirmPassword, ...registrationData } = formData;
      const result = await register(registrationData);

      if (result.success) {
        navigate("/dashboard");
      } else {
        setErrors({ general: result.message });
      }
    } catch (err) {
      setErrors({ general: "An unexpected error occurred. Please try again." });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      <div className="container">
        <div className="row justify-content-center">
          <div className="col-md-8 col-lg-6">
            <div className="auth-card card shadow-lg">
              <div className="card-body p-5">
                <div className="text-center mb-4">
                  <h2 className="fw-bold all-services">Create Account</h2>
                  <p className="text-muted">Join AutoCare today</p>
                </div>

                {errors.general && (
                  <div className="alert alert-danger" role="alert">
                    <i className="fas fa-exclamation-circle me-2"></i>
                    {errors.general}
                  </div>
                )}

                <form onSubmit={handleSubmit}>
                  <div className="row">
                    <div className="col-md-6 mb-3">
                      <label htmlFor="fullName" className="form-label">
                        Full Name
                      </label>
                      <input
                        type="text"
                        className={`form-control ${
                          errors.fullName ? "is-invalid" : ""
                        }`}
                        id="fullName"
                        name="fullName"
                        value={formData.fullName}
                        onChange={handleChange}
                        placeholder="John Doe"
                      />
                      {errors.fullName && (
                        <div className="invalid-feedback">
                          {errors.fullName}
                        </div>
                      )}
                    </div>

                    <div className="col-md-6 mb-3">
                      <label htmlFor="email" className="form-label">
                        Email Address
                      </label>
                      <input
                        type="email"
                        className={`form-control ${
                          errors.email ? "is-invalid" : ""
                        }`}
                        id="email"
                        name="email"
                        value={formData.email}
                        onChange={handleChange}
                        placeholder="john@example.com"
                      />
                      {errors.email && (
                        <div className="invalid-feedback">{errors.email}</div>
                      )}
                    </div>
                  </div>

                  <div className="row">
                    <div className="col-md-6 mb-3">
                      <label htmlFor="password" className="form-label">
                        Password
                      </label>
                      <input
                        type="password"
                        className={`form-control ${
                          errors.password ? "is-invalid" : ""
                        }`}
                        id="password"
                        name="password"
                        value={formData.password}
                        onChange={handleChange}
                        placeholder="••••••••"
                      />
                      {errors.password && (
                        <div className="invalid-feedback">
                          {errors.password}
                        </div>
                      )}
                    </div>

                    <div className="col-md-6 mb-3">
                      <label htmlFor="confirmPassword" className="form-label">
                        Confirm Password
                      </label>
                      <input
                        type="password"
                        className={`form-control ${
                          errors.confirmPassword ? "is-invalid" : ""
                        }`}
                        id="confirmPassword"
                        name="confirmPassword"
                        value={formData.confirmPassword}
                        onChange={handleChange}
                        placeholder="••••••••"
                      />
                      {errors.confirmPassword && (
                        <div className="invalid-feedback">
                          {errors.confirmPassword}
                        </div>
                      )}
                    </div>
                  </div>

                  <div className="row">
                    <div className="col-md-6 mb-3">
                      <label htmlFor="phoneNumber" className="form-label">
                        Phone Number
                      </label>
                      <input
                        type="tel"
                        className={`form-control ${
                          errors.phoneNumber ? "is-invalid" : ""
                        }`}
                        id="phoneNumber"
                        name="phoneNumber"
                        value={formData.phoneNumber}
                        onChange={handleChange}
                        placeholder="01012345678"
                      />
                      {errors.phoneNumber && (
                        <div className="invalid-feedback">
                          {errors.phoneNumber}
                        </div>
                      )}
                    </div>

                    <div className="col-md-6 mb-3">
                      <label htmlFor="city" className="form-label">
                        City
                      </label>
                      <input
                        type="text"
                        className={`form-control ${
                          errors.city ? "is-invalid" : ""
                        }`}
                        id="city"
                        name="city"
                        value={formData.city}
                        onChange={handleChange}
                        placeholder="Cairo"
                      />
                      {errors.city && (
                        <div className="invalid-feedback">{errors.city}</div>
                      )}
                    </div>
                  </div>

                  <div className="mb-3">
                    <label htmlFor="address" className="form-label">
                      Address
                    </label>
                    <input
                      type="text"
                      className={`form-control ${
                        errors.address ? "is-invalid" : ""
                      }`}
                      id="address"
                      name="address"
                      value={formData.address}
                      onChange={handleChange}
                      placeholder="123 Main Street"
                    />
                    {errors.address && (
                      <div className="invalid-feedback">{errors.address}</div>
                    )}
                  </div>

                  <button
                    type="submit"
                    className="btn btn-primary w-100 mb-3"
                    disabled={loading}>
                    {loading ? (
                      <>
                        <span
                          className="spinner-border spinner-border-sm me-2"
                          role="status"
                          aria-hidden="true"></span>
                        Creating Account...
                      </>
                    ) : (
                      "Create Account"
                    )}
                  </button>

                  <div className="text-center">
                    <p className="mb-0">
                      Already have an account?{" "}
                      <Link to="/login" className="text-white fw-bold">
                        Sign In
                      </Link>
                    </p>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register;
