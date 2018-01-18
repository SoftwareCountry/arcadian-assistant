import React, { Component } from 'react';
import { View, Text, ScrollView, Button, ActivityIndicator } from 'react-native';
import { NavigationScreenProps, NavigationActions } from 'react-navigation';
import { AppState } from '../reducers/app.reducer';
import { connect, Dispatch, MapStateToProps, MapDispatchToPropsFunction } from 'react-redux';
import { WithBackButtonProps, mapBackButtonDispatchToProps } from '../layout/back-button-dispatcher';
import { TicketTemplate } from '../reducers/helpdesk/ticket-template.model';
import { loadTicketTemplates } from '../reducers/helpdesk/tickets.actions';

interface HelpdeskScreenProps {
    ticketTemplates: TicketTemplate[];
    ticketTemplatesAreLoaded: boolean;
}

interface HelpdeskScreenDispatchProps extends WithBackButtonProps {
    requestTicketTemplates: () => void;
}

const mapStateToProps = (state: AppState): HelpdeskScreenProps => ({
    ticketTemplates: state.helpdesk.ticketTemplates,
    ticketTemplatesAreLoaded: !!state.helpdesk.ticketTemplates
});

const mapDispatchToProps = (dispatch: Dispatch<any>): HelpdeskScreenDispatchProps => ({
    ...mapBackButtonDispatchToProps(dispatch),
    requestTicketTemplates: () => dispatch(loadTicketTemplates()),
});

class HelpdeskScreenImpl extends Component<HelpdeskScreenProps & HelpdeskScreenDispatchProps> {
    public componentDidMount() {
        this.props.requestTicketTemplates();
    }

    public render() {

        const progressBar = this.props.ticketTemplatesAreLoaded ? undefined : <ActivityIndicator/>;

        return <ScrollView>
                {progressBar}
                <Button title='Back' onPress={ () => this.props.onBackClick() } ></Button>
            </ScrollView>;
    }
}

export const HelpdeskScreen = connect(mapStateToProps, mapDispatchToProps)(HelpdeskScreenImpl);