
export const fetchWithAuth = async (url, options = {}) => {
    const token = sessionStorage.getItem('token');

    if (!token) {
        logout();
        return Promise.reject(new Error('No token available'));
    }

    const defaultHeaders = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    };

    const response = await fetch(url, {
        ...options,
        headers: {
            ...defaultHeaders,
            ...options.headers
        }
    });

    if (response.status === 401) {
        logout();
        return Promise.reject(new Error('Unauthorized'));
    }

    return response;
};

