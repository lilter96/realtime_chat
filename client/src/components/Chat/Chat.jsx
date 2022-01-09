import React, { useState, useEffect } from 'react';
import { useLocation } from "react-router-dom";
import { HubConnectionBuilder } from '@microsoft/signalr';
import queryString from 'query-string';

import './Chat.css';
import InfoBar from '../InfoBar/InfoBar.jsx';
import Messages from '../Messages/Messages.jsx';
import Input from '../Input/Input';
import TextContainer from '../TextContainer/TextContainer';

const ENDPOINT = 'https://localhost:5001/hubs/chat';

const Chat = () => {
    const location = useLocation();
    const [connection, setConnection] = useState(null);
    const [name, setName] = useState('');
    const [room, setRoom] = useState('');
    const [message, setMessage] = useState([]);
    const [messages, setMessages] = useState([]);
    const [users, setUsers] = useState([]);

    useEffect(() => {
        const { name, room } = queryString.parse(location.search);
        setName(name);
        setRoom(room);

        const newConnection = new HubConnectionBuilder()
            .withUrl(ENDPOINT)
            .withAutomaticReconnect()
            .build();

        newConnection.start()
            .then(result => {
                console.log('Connected!')
                
                newConnection.on('ReceiveMessage', (message) => {
                    setMessages((messages) => [...messages, message]);
                });

                newConnection.on('ReceiveRoomInfo', (roomInfo) => {
                    setUsers(roomInfo.userNames)
                })

                setConnection(newConnection);
            })

        return () => {
            leaveRoom();
            connection.stop()
        }
    }, []);

    useEffect(() => {
        if (connection) joinRoom()
    }, [connection])

    const joinRoom = async () => {
        console.log(connection.connection.connectionId)
        const data = {
            roomName: room,
            connectionId: connection.connection.connectionId,
            userName: name
        }
        
        try {
            await fetch("https://localhost:5001/chat/room/join", {
                method: "POST",
                body: JSON.stringify(data),
                headers: {
                    'Content-Type': 'application/json'
                }
            });
        }
        catch (e)
        {
            console.log("Joining room failed!", e);
        }
    }

    const leaveRoom = () => {
        debugger
        console.log(connection.connection.connectionId)
        const data = {
            roomName: room,
            connectionId: connection.connection.connectionId,
            userName: name
        }
        
        try {
            fetch("https://localhost:5001/chat/room/leave", {
                method: "POST",
                body: JSON.stringify(data),
                headers: {
                    'Content-Type': 'application/json'
                }
            });
        }
        catch (e)
        {
            console.log("Leaving room failed!", e);
        }
    }

    const sendMessage = async (event) => {
        event.preventDefault();

        const chatMessage = {
            connectionId: connection.connection.connectionId,
            message: message
        }

        try {
            await fetch("https://localhost:5001/chat/message", {
                method: "POST",
                body: JSON.stringify(chatMessage),
                headers: {
                    'Content-Type': 'application/json'
                }
            });
        }
        catch (e) {
            console.log('Sending message failed!', e);
        }
    }

    return (
        <div className="outerContainer">
            <div className="container">
                <InfoBar room={room}/>
                <Messages messages={messages} name={name}/>
                <Input message={message} setMessage={setMessage} sendMessage={sendMessage}/>
            </div>
            <TextContainer users={users}/>
        </div>
    );
}

export default Chat;