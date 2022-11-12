import http from 'k6/http';
import { randomString } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

/* export const options = {
  vus: 50, // number of virtual users that will send requests
  duration: '30s', // how long will they be sending requests
}; */

export default function () {
  http.get('https://localhost:7267/posts?pagesize=25&pagenumber=300');
}