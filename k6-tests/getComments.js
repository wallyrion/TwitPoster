import http from 'k6/http';

const baseUrl = 'https://localhost:7267';

export const options = {
  vus: 50, // number of virtual users that will send requests
  iterations: 10000
};

export default function () {
  http.get(`${baseUrl}/posts/8/comments?pageNumber=2&pageSize=200`);
}