import http from 'k6/http';
import { randomString } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';
import { randomItem } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

export const options = {
  vus: 1000, // number of virtual users that will send requests
  duration: '5m', // how long will they be sending requests
};

function setupUser() {
  const randomFirstName = randomString(8);
  const email = `${randomFirstName}.2022-11-12T17:49:50.431Z@test.com`

  const user = {
    firstName: randomFirstName,
    lastName: randomString(8),
    birthDate: "1961-08-10T16:15:14.5204741+03:00",
    email: email,
    password: "Qwerty123!"
  }

  const result = http.post('https://localhost:7267/auth/registration', JSON.stringify(user), {
    headers: {
      'content-type': 'application/json'
    }
  })

  const response = JSON.parse(result.body);

  return response;
}

function setupUsers (count){
  let arr = [];

  for (let i = 0; i < count; i ++){
    const response = setupUser();

    arr.push(response);
  }

  return arr;
}

export function setup() {
  const users = setupUsers(10);
  
  return users
}


export default function (users) {

  const post = {
    body: randomString(5000)
  }

  const randomUser = randomItem(users);

  const result = http.post('https://localhost:7267/posts', JSON.stringify(post), {
    headers: {
      'content-type': 'application/json',
      'authorization': `Bearer ${randomUser.accessToken}`
    }
  })
}