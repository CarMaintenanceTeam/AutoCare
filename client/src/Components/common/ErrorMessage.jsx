import React from 'react';

const ErrorMessage = ({ message, onRetry }) => {
  return (
    <div className="alert alert-danger d-flex align-items-center" role="alert">
      <i className="fas fa-exclamation-circle me-3"></i>
      <div className="flex-grow-1">
        {message || 'An error occurred. Please try again.'}
      </div>
      {onRetry && (
        <button className="btn btn-sm btn-outline-danger ms-3" onClick={onRetry}>
          <i className="fas fa-redo me-1"></i> Retry
        </button>
      )}
    </div>
  );
};

export default ErrorMessage;
