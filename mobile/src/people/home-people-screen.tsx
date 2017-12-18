import { StackNavigator } from 'react-navigation';
import React from 'react';
import { Text, View, Platform, StatusBar } from 'react-native';
import { TopNavBar } from '../topNavBar/top-nav-bar';

const navBar =  new TopNavBar('People');
export class HomePeopleScreen extends React.Component {
    public static navigationOptions = navBar.configurate();

    public render() {
        return (
            <Text>PeopleScreen</Text>
        );
    }
}