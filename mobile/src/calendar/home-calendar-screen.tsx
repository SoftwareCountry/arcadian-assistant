import React, { Component } from 'react';
import { SafeAreaView, View } from 'react-native';
import styles from '../layout/styles';
import { Calendar } from './calendar';
import { Agenda } from './agenda';
import { DaysCounters } from './days-counters/days-counters';

//============================================================================
export class CalendarScreenImpl extends Component {
    public static navigationOptions = {
        headerStyle: {
            backgroundColor: '#2FAFCC'
        }
    };

    public render() {
        return <SafeAreaView style={{ flex: 1, backgroundColor: '#fff', }}>
            <View style={styles.container}>
                <DaysCounters/>
                <Calendar/>
                <Agenda/>
            </View>
        </SafeAreaView>;
    }
}
