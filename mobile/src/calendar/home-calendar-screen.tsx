import React, { Component } from 'react';
import { SafeAreaView, View } from 'react-native';
import { Calendar } from './calendar';
import { Agenda } from './agenda';
import { DaysCounters } from './days-counters/days-counters';
import Style from '../layout/style';

//============================================================================
export class CalendarScreenImpl extends Component {
    //----------------------------------------------------------------------------
    public static navigationOptions = {
        headerStyle: {
            backgroundColor: Style.color.base
        }
    };

    //----------------------------------------------------------------------------
    public render() {
        return <SafeAreaView style={Style.view.safeArea}>
            <View style={Style.view.container}>
                <DaysCounters/>
                <Calendar/>
                <Agenda/>
            </View>
        </SafeAreaView>;
    }
}
