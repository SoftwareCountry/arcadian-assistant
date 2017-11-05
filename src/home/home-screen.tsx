import React, { Component } from 'react';
import { NavigationTabScreenOptions } from 'react-navigation';
import { View, Text } from 'react-native';
import Icon from 'react-native-vector-icons/Ionicons';
import { TabNavigationOptionsFactory } from '../layout/tab-navigation-options-factory';
import styles from '../layout/styles';

const navOptionsFactory = new TabNavigationOptionsFactory();

export class HomeScreen extends Component {

    public static navigationOptions: NavigationTabScreenOptions =
    navOptionsFactory.create('Home', 'ios-home', 'ios-home-outline');

    public render() {
        return <View style={styles.container}>
            <Text>Home</Text>
        </View>;
    }
}