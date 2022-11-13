import http from 'k6/http';
import { randomString } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';
import { randomItem } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';
import { randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';

const baseUrl = 'https://localhost:7267'
const numberOfUsersToSetup = 10000;
const likesToSetupBetween = [5, 500];
const commentsToSetupBetween = [1, 100];

const inerations = 1000;
const numberOfCuncurrentUsers = 100;

export const options = {
  vus: numberOfCuncurrentUsers, // number of virtual users that will send requests
  //duration: '3m', // how long will they be sending requests
  iterations: inerations,
  duration: '30m'
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

  const result = http.post(`${baseUrl}/auth/registration`, JSON.stringify(user), {
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
  const users = setupUsers(numberOfUsersToSetup);
  
  return users
}


export default function (users) {
  const post = {
    body: randomString(5000)
  }

  const randomUser = randomItem(users);

  const createdPostResponse = http.post(`${baseUrl}/posts`, JSON.stringify(post), {
    headers: {
      'content-type': 'application/json',
      'authorization': `Bearer ${randomUser.accessToken}`
    }
  })

  const likesCount = randomIntBetween(likesToSetupBetween[0], likesToSetupBetween[1])
  const commentsCount = randomIntBetween(commentsToSetupBetween[0], commentsToSetupBetween[1])

  const createdPost = JSON.parse(createdPostResponse.body);
  console.log ('Setting up ' + likesCount + ' likes for post id = ' + createdPost.id)


  for (let i = 0; i < likesCount; i ++){
    createLikesForPost(createdPost.id, users)
  }

  console.log ('Setting up ' + commentsCount + ' comments for post id = ' + createdPost.id)
  for (let i = 0; i < commentsCount; i ++){
    createCommentForPost(createdPost.id, users)
  }
}

function createLikesForPost(postId, users){
  const randomUser = randomItem(users);

  const createdPost = http.put(`${baseUrl}/posts/${postId}/like`, null, {
    headers: {
      'content-type': 'application/json',

      'authorization': `Bearer ${randomUser.accessToken}`
    }
  })
}

function createCommentForPost(postId, users){
  const randomUser = randomItem(users);

  const commentBody = { text: randomString(1000)}
  const createdPost = http.post(`${baseUrl}/posts/${postId}/comments`, JSON.stringify(commentBody), {
    headers: {
      'content-type': 'application/json',

      'authorization': `Bearer ${randomUser.accessToken}`
    }
  })
}