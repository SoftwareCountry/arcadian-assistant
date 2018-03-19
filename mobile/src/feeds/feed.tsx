import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image, LayoutChangeEvent, TouchableOpacity } from 'react-native';
import { LayoutEvent } from 'react-navigation';

import { Avatar } from '../people/avatar';
import { Feed } from '../reducers/feeds/feed.model';
import { feedStyles as styles } from './styles';
import { Employee } from '../reducers/organization/employee.model';
import { StyledText } from '../override/styled-text';
import { Dispatch, connect } from 'react-redux';

interface FeedListItemProps {
    message: Feed;
    employee: Employee;
    onAvatarClicked: (e: Employee) => void;
}

interface FeedListItemState {
    avatarHeight: number;
}

export class FeedListItem extends React.Component<FeedListItemProps, FeedListItemState> {
    constructor(props: FeedListItemProps) {
        super(props);
        this.state = {
            avatarHeight: undefined
        };
    }

    public render() {
        const message = this.props.message;
        const employee = this.props.employee;

        const employeeName = employee ? employee.name : message.title;
        const photo = employee ? employee.photo : null;

        const formattedDate = message.datePosted.format('MMMM D, YYYY');

        const imgContainerStyle = StyleSheet.flatten([
            styles.imgContainer,
            this.state.avatarHeight
                ? { height: this.state.avatarHeight }
                : {}
        ]);

        return (
            <TouchableHighlight>
                <View style={styles.layout}>
                    <View style={imgContainerStyle} onLayout={this.onAvatarContainerLayout}>
                        <TouchableOpacity onPress = {this.onAvatarClicked} style={styles.touchableOpacityContainer}>
                            <Avatar photo={photo} />
                        </TouchableOpacity>
                    </View>
                    <View style={styles.info}>
                        <StyledText style={styles.title}>{employeeName}</StyledText>
                        <StyledText>
                            <Text style={styles.to}>@Arcadians</Text>
                            <Text style={styles.text}>, {this.props.message.text}</Text>
                        </StyledText>
                        <StyledText style={styles.tags}>#ArcadiaNews</StyledText>
                        <StyledText style={styles.date}>{formattedDate}</StyledText>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }

    private onAvatarContainerLayout = (evt: LayoutChangeEvent) => {
        const layout = evt.nativeEvent.layout;
        this.setState({
            avatarHeight: Math.max(layout.height, layout.width)
        });
    }
    private onAvatarClicked = () => this.props.onAvatarClicked(this.props.employee);
}
