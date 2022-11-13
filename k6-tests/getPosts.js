import { sleep } from 'k6';
import http from 'k6/http';
import { randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

const pageSize = 100;
const baseUrl = 'https://localhost:7267'

export const options = {
    stages: [
        { duration: '30s', target: 50 }, 
        { duration: '1m', target: 300 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
        { duration: '3m', target: 300 }, // stay at 100 users for 10 minutes
        { duration: '1m', target: 0 }, // ramp-down to 0 users
      ],
      thresholds: {
        'http_req_duration': ['p(99)<1500'], // 99% of requests must complete below 1.5s
      },
  };

export function setup() {
    const res = http.get(`${baseUrl}/posts/count`);

    const postsCount = JSON.parse(res.body);

    console.log(postsCount);

    const pages = Math.floor(postsCount / pageSize);

    console.log('pages', pages)
    return pages;
  }


export default function (pages) {

  const page = randomIntBetween(1, pages);

  const query = `${baseUrl}/posts?pageSize=${pageSize}&pageNumber=${page}`;
  http.get(query);
}