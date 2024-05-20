import random
import socket
import threading
from time import sleep

server_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
server_socket.bind(('127.0.0.1', 8888))
ads = []

def lisnter():
    while True:
        message, address = server_socket.recvfrom(1024)
        print(message.__str__())
        if(message == b"r"):
            print("start at,",address.__str__())
            ads.append(address)
        if(message == b"s"):
            print("close at,",address.__str__())
            ads.remove(address)

def worker():
    while True:
        for address in ads: 
            print("sended")
            server_socket.sendto((f"""m:1:1:1:{random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}#
                                  {random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}#
                                  {random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}:{random.randint(0, 10)}#""").encode('utf-8'), address)
        sleep(1) 

threading.Thread(target=lisnter).start()
threading.Thread(target=worker).start()