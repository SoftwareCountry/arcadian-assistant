import React, { Component } from 'react';
import { View, Text } from 'react-native';
import styles from '../layout/styles';
import { calendarScreenLayout } from './styles';
import { Calendar } from './calendar';
import { Agenda } from './agenda';
import { TopNavBar } from '../navigation/top-nav-bar';

const navBar =  new TopNavBar('Calendar');

export class CalendarScreenImpl extends Component {
    public static navigationOptions = navBar.configurate();

    public render() {
        return <View style={styles.container}>
            <Calendar />
            <Agenda />
        </View>;
    }
}
