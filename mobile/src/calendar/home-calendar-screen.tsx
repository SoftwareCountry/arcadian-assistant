import React, { Component } from 'react';
import { View, Text, Platform } from 'react-native';
import styles from '../layout/styles';
import { calendarScreenLayout } from './styles';
import { Calendar } from './calendar';
import { Agenda } from './agenda';
import { TopNavBar } from '../navigation/top-nav-bar';
import { DaysCounters } from './days-counters/days-counters';

const navBar =  new TopNavBar('');

export class CalendarScreenImpl extends Component {
    public static navigationOptions = {
        headerTitle: 'Calendar',
        headerTitleStyle: {
            fontFamily: 'CenturyGothic',
            fontSize: 14,
            color: 'white',
            textAlign: 'center'
        },
        headerStyle: {
            height: Platform.OS === 'ios' ? 50 : 30,
            marginTop: 0,
            backgroundColor: '#2FAFCC'
        }
    };

    public render() {
        return <View style={styles.container}>
            <DaysCounters />
            <Calendar />
            <Agenda />
        </View>;
    }
}
