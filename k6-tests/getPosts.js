import { sleep } from 'k6';
import http from 'k6/http';


export const options = {
    /* vus: 50, // number of virtual users that will send requests
    //iterations: 10000
    duration: '30s' */

    stages: [
        { duration: '5m', target: 100 }, // simulate ramp-up of traffic from 1 to 100 users over 5 minutes.
        { duration: '10m', target: 100 }, // stay at 100 users for 10 minutes
        { duration: '5m', target: 0 }, // ramp-down to 0 users
      ],
};


export default function () {
  http.get('https://localhost:7267/posts/sync');
}