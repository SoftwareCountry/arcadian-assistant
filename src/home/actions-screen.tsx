import React, { Component } from 'react';
import { View, Text, ScrollView, Button } from 'react-native';
import { NavigationScreenProps, NavigationActions } from 'react-navigation';
import { AppState } from '../reducers/app.reducer';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';

interface HelpdeskScreenProps {
    messageTemplates: {}[];
    onBackPress: () => void;
}

const mapStateToProps = (state: AppState, ownProps: any) => ({
    ...ownProps,
    messageTemplates: state.helpdesk.messageTemplates
});

const mapDispatchToProps = (dispatch: Dispatch<any>) => ({
    onBackPress: () => dispatch(NavigationActions.back())
});

class ActionsScreenImpl extends Component<HelpdeskScreenProps> {
    public render() {

        console.log(this.props);

        return <ScrollView>
                <Text>Si!</Text>
                <Button title='Back' onPress={ () => this.props.onBackPress() } ></Button>
            </ScrollView>;
    }
}

//export const ActionsScreen = ActionsScreenImpl;

export const ActionsScreen = connect(mapStateToProps, mapDispatchToProps)(ActionsScreenImpl);