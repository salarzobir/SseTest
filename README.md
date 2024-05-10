# SseTest

## Overview

This repository is dedicated to demonstrating and resolving issues encountered with Server-Sent Events (SSE) when used in conjunction with .NET's built-in event handling system.

## Problem Description

The application is designed to utilize SSE to broadcast server-side events. However, several challenges have arisen with the current implementation:

1. **Crash on POST Request:** When a POST request is made, an exception is thrown, causing the program to crash unexpectedly.

2. **Concatenation of Messages:** Implementing a fire-and-forget approach within the event delegate prevents the application from crashing, but it results in the unintended concatenation of messages.

3. **Async Locking with SemaphoreSlim:** To manage the concurrency of message sending, `SemaphoreSlim` was introduced in the `SseClient.SendAsync` method for asynchronous locking. While this has helped manage access to resources, I am concerned that there might be a more efficient or correct way to handle this.

## Constraints

- The use of built-in .NET events is essential as the application relies on a third-party library that uses this event-driven model.