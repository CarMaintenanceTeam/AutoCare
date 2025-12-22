import React from 'react';

const Loading = ({ message = 'Loading...' }) => {
  return (
    <div className="d-flex flex-column justify-content-center align-items-center" style={{ minHeight: '300px' }}>
      <div className="spinner-border text-primary mb-3" role="status">
        <span className="visually-hidden">Loading...</span>
      </div>
      <p className="text-muted">{message}</p>
    </div>
  );
};

export default Loading;
